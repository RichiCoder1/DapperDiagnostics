using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace DapperDiagnostics.Analyzers.Shared
{
    internal static class SqlHelpers
    {
        private static readonly Regex ParameterRegex = new Regex(":[a-zA-Z_]*");

        internal static IImmutableList<string> GetSqlParameters(string sqlText)
        {
            var parameterMatches = ParameterRegex.Matches(sqlText);

            return parameterMatches.Cast<Match>().Select(match => match.Value.Trim().TrimStart(':')).ToImmutableList();
        }
    }
}
