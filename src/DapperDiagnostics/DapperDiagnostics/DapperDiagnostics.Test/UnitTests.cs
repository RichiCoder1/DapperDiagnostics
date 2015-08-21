using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using DapperDiagnostics;
using DapperDiagnostics.Analyzers;

// Will come back to these later.
namespace DapperDiagnostics.Test
{
    //[TestClass]
    public class UnitTest : DiagnosticVerifier
    {

    //    //No diagnostics expected to show up
    //    [TestMethod]
    //    public void TestMethod1()
    //    {
    //        var test = @"";

    //        VerifyCSharpDiagnostic(test);
    //    }

    //    //Diagnostic and CodeFix both triggered and checked for
    //    [TestMethod]
    //    public void TestMethod2()
    //    {
    //        var test = @"
    //using System;
    //using System.Collections.Generic;
    //using System.Linq;
    //using System.Text;
    //using System.Threading.Tasks;
    //using System.Diagnostics;

    //namespace ConsoleApplication1
    //{
    //    class TypeName
    //    {   
    //    }
    //}";
    //        var expected = new DiagnosticResult
    //        {
    //            Id = "DapperDiagnostics",
    //            Message = String.Format("Type name '{0}' contains lowercase letters", "TypeName"),
    //            Severity = DiagnosticSeverity.Warning,
    //            Locations =
    //                new[] {
    //                        new DiagnosticResultLocation("Test0.cs", 11, 15)
    //                    }
    //        };

    //        VerifyCSharpDiagnostic(test, expected);

    //        var fixtest = @"
    //using System;
    //using System.Collections.Generic;
    //using System.Linq;
    //using System.Text;
    //using System.Threading.Tasks;
    //using System.Diagnostics;

    //namespace ConsoleApplication1
    //{
    //    class TYPENAME
    //    {   
    //    }
    //}";
    //        VerifyCSharpFix(test, fixtest);
    //    }

    //    protected override CodeFixProvider GetCSharpCodeFixProvider()
    //    {
    //        return new DapperDiagnosticsCodeFixProvider();
    //    }

    //    protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
    //    {
    //        return new DapperDiagnosticsAnalyzer();
    //    }
    }
}