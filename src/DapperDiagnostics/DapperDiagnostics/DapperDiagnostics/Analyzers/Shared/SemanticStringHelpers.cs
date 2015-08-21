using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DapperDiagnostics.Analyzers.Shared
{
    internal static class SemanticStringHelpers
    {
        internal static string GetResolvedString(SyntaxNodeAnalysisContext context, ExpressionSyntax expression, ref bool isNotConstant)
        {
            if (expression == null)
                return string.Empty;

            switch (expression.Kind())
            {
                case SyntaxKind.AddExpression:
                    var addExpression = (BinaryExpressionSyntax)expression;
                    return GetResolvedString(context, addExpression.Left, ref isNotConstant) + GetResolvedString(context, addExpression.Right, ref isNotConstant);
                case SyntaxKind.IdentifierName:
                    var value = context.SemanticModel.GetConstantValue(expression);
                    return value.HasValue ? value.Value as string : string.Empty;
                case SyntaxKind.StringLiteralExpression:
                    var literalExpression = (LiteralExpressionSyntax)expression;
                    return literalExpression.Token.Value as string;
                default:
                    isNotConstant = true;
                    return string.Empty;
            }
        }
    }
}
