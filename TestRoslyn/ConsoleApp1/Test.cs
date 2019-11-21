using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Xu;

namespace TestRoslyn
{
    public class SampleTest
    {
        public SampleTest(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;

        public override string ToString() => (X + Y).ToString();
    }

    public static class Test
    {
        public static async Task SimpleAsync()
        {
            try
            {
                int result = await CSharpScript.EvaluateAsync<int>("1 + 2");

                Console.WriteLine(result);
                //SampleTest pf = new SampleTest(12, 56); Console.WriteLine(pf.ToString());
                //Period pf = new Period(DateTime.Now.AddDays(-1), DateTime.Now);
                //Console.WriteLine(pf.ToString());

                string scp = "using System; using Xu;";
                //await CSharpScript.EvaluateAsync("using System; Console.WriteLine(\"Hello world!\");");
                //await CSharpScript.EvaluateAsync(scp + "Period pf = new Period(DateTime.Now.AddDays(-1), DateTime.Now); " + "Console.WriteLine(pf.ToString()); ", ScriptOptions.Default.WithReferences(typeof(Period).Assembly));
                //await CSharpScript.EvaluateAsync(scp + "Period pf = new Period(DateTime.Now.AddDays(-1), DateTime.Now); " + "Console.WriteLine(pf.ToString()); ", ScriptOptions.Default.WithImports("Xu"));
                await CSharpScript.EvaluateAsync(scp + "Period pf = new Period(DateTime.Now.AddDays(-1), DateTime.Now); " + "Console.WriteLine(pf.ToString()); ", ScriptOptions.Default.WithReferences(Assembly.LoadFrom("Xu.dll"))); // Assembly.GetExecutingAssembly()

                // https://blogs.msdn.microsoft.com/cdndevs/2015/12/01/adding-c-scripting-to-your-development-arsenal-part-1/

                // https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples

                //var result2 = CSharpScript.EvaluateAsync<int>(script, null, new ScriptHost { Number = 5 }).Result;
                //var script = CSharpScript.Create<int>("X*Y", globalsType: typeof(Globals));

                //string scp = "using System; using TestRoslyn;";
                //await CSharpScript.EvaluateAsync(scp + "SampleTest pf = new SampleTest(12, 56); " + "Console.WriteLine(pf.ToString()); ", ScriptOptions.Default.WithReferences(typeof(SampleTest).Assembly));//, ScriptOptions.Default.WithImports("Xu.dll"));
                //await CSharpScript.EvaluateAsync(scp + "SampleTest pf = new SampleTest(12, 56); " + "Console.WriteLine(pf.ToString()); ", ScriptOptions.Default.WithReferences(typeof(SampleTest).Assembly));
                //await CSharpScript.EvaluateAsync(scp + "SampleTest pf = new SampleTest(12, 56); " + "Console.WriteLine(pf.ToString()); ", ScriptOptions.Default.WithReferences(Assembly.GetExecutingAssembly()));
            }
            catch (CompilationErrorException e)
            {
                Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
            }
        }

    }

    public class CSharpScriptEngine
    {
        private static ScriptState scriptState = null;
        public static object Execute(string code)
        {
            scriptState = scriptState == null ? CSharpScript.RunAsync(code).Result : scriptState.ContinueWithAsync(code).Result;
            if (scriptState.ReturnValue != null && !string.IsNullOrEmpty(scriptState.ReturnValue.ToString()))
                return scriptState.ReturnValue;
            return null;
        }
    }

    //This code utilizes the Microsoft.CodeAnalysis.CSharp.Scripting package available at NuGet.
    //The `CSharpScript.RunAsync` method runs a C# script while `scriptState.ContinueWithAsync` continues the previously submitted code with the current submitted one.
    //With this in place we can try out the script engine:

    class Program2
    {
        static void Main(string[] args)
        {
            CSharpScriptEngine.Execute(
                //This could be code submitted from the editor
                @"
            public class ScriptedClass
            {
                public String HelloWorld {get;set;}
                public ScriptedClass()
                {
                    HelloWorld = ""Hello Roslyn!"";
                }
            }");
            //And this from the REPL
            Console.WriteLine(CSharpScriptEngine.Execute("new ScriptedClass().HelloWorld"));
            Console.ReadKey();
        }
    }
    //Output: "Hello Roslyn!"
}
