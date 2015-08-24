using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

using DapperDiagnostics.Analyzers.Shared;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DapperDiagnostics.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReturnObjectMismatchAnalyzer : DiagnosticAnalyzer
    {
        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ReturnObjectMismatchTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString ScalarMessageFormat = new LocalizableResourceString(nameof(Resources.ReturnObjectMismatch_Scalar_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString ScalarDescription = new LocalizableResourceString(nameof(Resources.ReturnObjectMismatch_Scalar_Description), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString PropertiesMessageFormat = new LocalizableResourceString(nameof(Resources.ReturnObjectMismatch_Properties_MessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString PropertiesDescription = new LocalizableResourceString(nameof(Resources.ReturnObjectMismatch_Properties_Description), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor ScalarRule = new DiagnosticDescriptor(DiagnosticIds.ReturnObjectMismatchRuleId, Title, ScalarMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: ScalarDescription);
        private static readonly DiagnosticDescriptor PropertiesRule = new DiagnosticDescriptor(DiagnosticIds.ReturnObjectMismatchRuleId, Title, PropertiesMessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: PropertiesDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ScalarRule, PropertiesRule);

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
            var sqlArgument = invocation.ArgumentList.Arguments.FirstOrDefault();
            if (sqlArgument == null) return;

            var sqlLiteral = SemanticStringHelpers.GetResolvedString(context, sqlArgument.Expression, ref isNotConstant);
            if (string.IsNullOrWhiteSpace(sqlLiteral)) return;

            // Do we have a select statement?
            if (sqlLiteral.IndexOf("select", StringComparison.OrdinalIgnoreCase) < 0) return;

            var genericTypeSyntax = ((invocation.Expression as MemberAccessExpressionSyntax)?.Name as GenericNameSyntax)?.TypeArgumentList.Arguments.FirstOrDefault();
            if (genericTypeSyntax == null) return;

            var genericTypeInfo = context.SemanticModel.GetTypeInfo(genericTypeSyntax);
            var genericTypeArgument = genericTypeInfo.ConvertedType;
            if (genericTypeArgument == null) return;

            bool isValid;
            var selectNames = SqlHelpers.GetSqlSelectValues(sqlLiteral, out isValid);
            if (!isValid) return;

            // If we're expecting a scalar type, but returning multiple things, that's wrong.
            if (TypeHelpers.IsPrimitiveType(genericTypeArgument) || TypeHelpers.IsString(genericTypeArgument))
            {
                if (selectNames.Count == 1) return;

                var diagnostic = Diagnostic.Create(
                    ScalarRule,
                    sqlArgument.GetLocation(),
                    selectNames.Count);
                context.ReportDiagnostic(diagnostic);
            }
            else
            {
                var availableProperties = TypeHelpers.GetMembers(genericTypeArgument);

                if (selectNames.Any(
                    name => !availableProperties.Contains(name, StringComparer.CurrentCultureIgnoreCase)))
                {
                    var diagnostic = Diagnostic.Create(
                        PropertiesRule,
                        sqlArgument.GetLocation(),
                        string.Join(", ", selectNames.Where(name => !availableProperties.Contains(name, StringComparer.CurrentCultureIgnoreCase))),
                        genericTypeArgument.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
