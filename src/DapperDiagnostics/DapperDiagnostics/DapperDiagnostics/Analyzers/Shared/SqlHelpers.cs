using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

using PLSQL.Grammer;

namespace DapperDiagnostics.Analyzers.Shared
{
    internal static class SqlHelpers
    {
        private static readonly Regex ParameterRegex = new Regex(":[a-zA-Z_]*");

        internal static IImmutableList<string> GetSqlParameters(string sqlText, out bool isValid)
        {
            isValid = true;
            try
            {
                var input = new AntlrInputStream(sqlText);
                var lexer = new PLSQLLexer(input);
                var tokens = new CommonTokenStream(lexer);
                var parser = new PLSQLParser(tokens);
                var tree = parser.Parse();
                var visitor = new SqlSelectVisitor();
                return visitor.Visit(tree);
            }
            catch (ParseCanceledException)
            {
                isValid = false;
                return ImmutableList<string>.Empty;
            }
        }

        private class SqlSelectVisitor : PLSQLBaseVisitor<IImmutableList<string>>
        {
            #region Overrides of PLSQLBaseVisitor<IImmutableList<string>>

            public override IImmutableList<string> VisitResult_column(PLSQLParser.Result_columnContext context)
            {
                var alias = context.column_alias();
                if (alias != null)
                {
                    return ImmutableList.Create(alias.GetText());
                }

                var column = context.expr().column_name();
                if (column != null)
                {
                    return ImmutableList.Create(column.GetText());
                }

                return DefaultResult;
            }

            #endregion

            #region Overrides of AbstractParseTreeVisitor<IImmutableList<string>>

            /// <inheritdoc />
            protected override IImmutableList<string> DefaultResult => ImmutableList<string>.Empty;

            /// <summary>
            /// Aggregates the results of visiting multiple children of a node.
            /// </summary>
            /// <remarks>
            /// Aggregates the results of visiting multiple children of a node. After
            ///             either all children are visited or
            ///             <see cref="M:Antlr4.Runtime.Tree.AbstractParseTreeVisitor`1.ShouldVisitNextChild(Antlr4.Runtime.Tree.IRuleNode,`0)"/>
            ///             returns
            /// <code>
            /// false
            /// </code>
            ///             , the aggregate value is returned as the result of
            ///             <see cref="M:Antlr4.Runtime.Tree.AbstractParseTreeVisitor`1.VisitChildren(Antlr4.Runtime.Tree.IRuleNode)"/>
            ///             .
            ///             <p>The default implementation returns
            /// <code>
            /// nextResult
            /// </code>
            ///             , meaning
            ///             <see cref="M:Antlr4.Runtime.Tree.AbstractParseTreeVisitor`1.VisitChildren(Antlr4.Runtime.Tree.IRuleNode)"/>
            ///             will return the result of the last child visited
            ///             (or return the initial value if the node has no children).</p>
            /// </remarks>
            /// <param name="aggregate">The previous aggregate value. In the default
            ///             implementation, the aggregate value is initialized to
            ///             <see cref="P:Antlr4.Runtime.Tree.AbstractParseTreeVisitor`1.DefaultResult"/>
            ///             , which is passed as the
            /// <code>
            /// aggregate
            /// </code>
            ///             argument
            ///             to this method after the first child node is visited.
            ///             </param><param name="nextResult">The result of the immediately preceeding call to visit
            ///             a child node.
            ///             </param>
            /// <returns>
            /// The updated aggregate result.
            /// </returns>
            protected override IImmutableList<string> AggregateResult(IImmutableList<string> aggregate, IImmutableList<string> nextResult)
            {
                if (aggregate.Count == 0 && nextResult.Count == 0) return DefaultResult;
                return aggregate.AddRange(nextResult);
            }

            #endregion
        }
    }
}
