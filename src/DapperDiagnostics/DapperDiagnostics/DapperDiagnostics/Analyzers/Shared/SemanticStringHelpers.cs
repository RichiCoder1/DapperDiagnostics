using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DapperDiagnostics.Analyzers.Shared
{
    internal static class SemanticStringHelpers
    {
        private static readonly ConditionalWeakTable<ExpressionSyntax, string> _conditionalWeakTable = 
            new ConditionalWeakTable<ExpressionSyntax, string>(); 

        internal static string GetResolvedString(SyntaxNodeAnalysisContext context, ExpressionSyntax expression, ref bool isNotConstant)
        {
            if (expression == null)
                return string.Empty;

            string result;
            if (_conditionalWeakTable.TryGetValue(expression, out result))
            {
                return result;
            }

            switch (expression.Kind())
            {
                case SyntaxKind.AddExpression:
                    var addExpression = (BinaryExpressionSyntax)expression;
                    result = GetResolvedString(context, addExpression.Left, ref isNotConstant) + GetResolvedString(context, addExpression.Right, ref isNotConstant);
                    break;
                case SyntaxKind.IdentifierName:
                    var value = context.SemanticModel.GetConstantValue(expression);
                    result = value.HasValue ? value.Value as string : string.Empty;
                    break;
                case SyntaxKind.StringLiteralExpression:
                    var literalExpression = (LiteralExpressionSyntax)expression;
                    result = literalExpression.Token.Value as string;
                    break;
                default:
                    isNotConstant = true;
                    result = string.Empty;
                    break;
            }

            _conditionalWeakTable.Add(expression, result);
            return result;
        }
    }
}
