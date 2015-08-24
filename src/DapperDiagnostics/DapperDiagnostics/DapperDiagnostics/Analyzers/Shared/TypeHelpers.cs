using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DapperDiagnostics.Analyzers.Shared
{
    internal static class TypeHelpers
    {

        public static IReadOnlyList<string> GetMembers(
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


        public static IEnumerable<string> GetMembers(
            ITypeSymbol symbol)
        {
            if (symbol == null) return Enumerable.Empty<string>();
            var properties = symbol.GetMembers()
                .Where(member => member.Kind == SymbolKind.Property && member.DeclaredAccessibility == Accessibility.Public)
                .Select(member => member.Name)
                .ToList();

            return properties.Union(GetMembers(symbol.BaseType));
        }

        public static bool IsPrimitiveType(ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Byte:
                case SpecialType.System_Char:
                case SpecialType.System_Double:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                case SpecialType.System_SByte:
                case SpecialType.System_Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsString(ITypeSymbol type)
        {
            switch (type.SpecialType)
            {
                case SpecialType.System_String:
                    return true;
                default:
                    return false;
            }
        }
    }
}
