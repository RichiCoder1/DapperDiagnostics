using System.Collections.Immutable;
using System.Linq;

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

using PLSQL.Grammer;

namespace DapperDiagnostics.Analyzers.Shared
{
    internal static class SqlHelpers
    {
        internal static IImmutableList<string> GetSqlParameters(string sqlText, out bool isValid)
        {
            var tree = Parse(sqlText, out isValid);
            if (!isValid) return ImmutableList<string>.Empty;

            var visitor = new SqlParameterVisitor();
            IImmutableList<string> results;
            isValid = visitor.TryVisit(tree, out results);
            return results
                .Select(item => item.TrimStart(':'))
                .Where(result => !string.IsNullOrWhiteSpace(result))
                .ToImmutableList();
        }

        internal static IImmutableList<string> GetSqlSelectValues(string sqlText, out bool isValid)
        {
            var tree = Parse(sqlText, out isValid);
            if (!isValid) return ImmutableList<string>.Empty;

            var visitor = new SqlSelectVisitor();
            IImmutableList<string> results;
            isValid = visitor.TryVisit(tree, out results);
            return results;
        }

        internal static bool TryParse(string sqlText, out IParseTree tree, out string error)
        {
            error = null;
            tree = null;
            try
            {
                
                var input = new AntlrInputStream(sqlText);
                var lexer = new PLSQLLexer(input);
                var tokens = new CommonTokenStream(lexer);
                var parser = new PLSQLParser(tokens);
                tree = parser.Parse();
                return true;
            }
            catch (ParseCanceledException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        internal static IParseTree Parse(string sqlText, out bool isValid)
        {
            IParseTree tree;
            string error;
            isValid = TryParse(sqlText, out tree, out error);
            return tree;
        }

        internal static bool Validate(IParseTree tree)
        {
            bool throwAway;
            return new PLSQSafeVisitor<bool>().TryVisit(tree, out throwAway);
        }

        private class SqlParameterVisitor : PLSQSafeVisitor<IImmutableList<string>>
        {
            #region Overrides of PLSQLBaseVisitor<IImmutableList<string>>

            public override IImmutableList<string> VisitBindExpr(PLSQLParser.BindExprContext context)
            {
                return ImmutableList.Create(context.GetText());
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

        private class SqlSelectVisitor : PLSQSafeVisitor<IImmutableList<string>>
        {
            #region Overrides of PLSQLBaseVisitor<IImmutableList<string>>

            public override IImmutableList<string> VisitResult_column(PLSQLParser.Result_columnContext context)
            {
                var alias = context.column_alias();
                if (alias != null)
                {
                    return ImmutableList.Create(alias.GetText());
                }

                var column = context.expr();
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
