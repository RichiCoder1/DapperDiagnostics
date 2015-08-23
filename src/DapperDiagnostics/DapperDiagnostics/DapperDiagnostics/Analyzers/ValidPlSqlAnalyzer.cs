using System.Collections.Immutable;

using Antlr4.Runtime.Tree;

using DapperDiagnostics.Analyzers.Shared;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DapperDiagnostics.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ValidPlSqlAnalyzer : DiagnosticAnalyzer
    {
        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.ValidPLSQLTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.ValidPLSQLMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.ValidPLSQLDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIds.MissingParamsPropertiesRuleId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

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
            if (!(symbol.ReceiverType.Name.EndsWith("IDbConnection"))) return;

            // Is our first argument a string aka SQL? Is the string a constant value (aka, does it contain anything besides string literals and const references)?
            var isNotConstant = false;
            var sqlLiteral = SemanticStringHelpers.GetResolvedString(context, invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression, ref isNotConstant);
            if (string.IsNullOrWhiteSpace(sqlLiteral) || isNotConstant) return;

            IParseTree tree;
            string error;
            if (!SqlHelpers.TryParse(sqlLiteral, out tree, out error) || !SqlHelpers.Validate(tree))
            {
                var diagnostic = Diagnostic.Create(Rule, invocation.ArgumentList.Arguments.First().GetLocation(), error);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
