using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using DapperDiagnostics.Analyzers.Shared;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DapperDiagnostics.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingParemetersAnalyzer : DiagnosticAnalyzer
    {
        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MissingParametersTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MissingParametersMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MissingParametersDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.MissingParemetersRuleId, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.InvocationExpression);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var invocation = (InvocationExpressionSyntax)context.Node;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);

            var symbol = symbolInfo.Symbol as IMethodSymbol;

            // Are we an extension method?
            if (symbol?.MethodKind != MethodKind.ReducedExtension) return;

            // Are we extending IDbConnection
            if (!(symbol?.ReceiverType.Name.EndsWith("IDbConnection")).GetValueOrDefault()) return;

            // Is our first argument a string aka SQL? Is the string a constant value (aka, does it contain anything besides string literals and const references)?
            var isNotConstant = false;
            var sqlLiteral = SemanticStringHelpers.GetResolvedString(context, invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression, ref isNotConstant);
            if (string.IsNullOrWhiteSpace(sqlLiteral)) return;

            var secondArgument = invocation.ArgumentList.Arguments.Skip(1).FirstOrDefault()?.Expression;
            if (secondArgument == null) return;

            IReadOnlyList<string> availableProperties;
            if (secondArgument is AnonymousObjectCreationExpressionSyntax)
            {
                availableProperties = ProcessAnonymousObject(secondArgument as AnonymousObjectCreationExpressionSyntax);
            }
            else if (secondArgument is IdentifierNameSyntax)
            {
                availableProperties = ProcessFormedObject(context, secondArgument as IdentifierNameSyntax);

            }
            else
            {
                return;
            }

            var parameters = SqlHelpers.GetSqlParameters(sqlLiteral);

            if (!availableProperties.All(
                    availableProperty => parameters.Contains(availableProperty, StringComparer.CurrentCultureIgnoreCase)))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    secondArgument.GetLocation(),
                    string.Join(
                        ", ",
                        availableProperties.Where(
                            availableProperty =>
                            !parameters.Contains(availableProperty, StringComparer.CurrentCultureIgnoreCase))));

                context.ReportDiagnostic(diagnostic);
            }
        }

        private static IReadOnlyList<string> ProcessAnonymousObject(
            AnonymousObjectCreationExpressionSyntax anonymousObject)
        {
            return anonymousObject.Initializers.Select(
                    declarator =>
                        declarator.NameEquals?.Name.Identifier.Text
                        ?? (declarator.Expression as IdentifierNameSyntax)?.Identifier.Text
                        ?? (declarator.Expression as MemberAccessExpressionSyntax)?.Name.Identifier.Text)
                    .Where(name => name != null)
                    .ToList();
        }

        private static IReadOnlyList<string> ProcessFormedObject(
            SyntaxNodeAnalysisContext context,
            IdentifierNameSyntax syntax)
        {
            return context.SemanticModel.GetTypeInfo(syntax)
                .Type.GetMembers()
                .Where(symbol => symbol.Kind == SymbolKind.Property)
                .Select(symbol => symbol.Name)
                .ToList();
        }
    }
}
