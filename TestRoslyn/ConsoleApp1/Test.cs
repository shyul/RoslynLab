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

    public class ScriptHost
    {
        public int Number { get; set; }
    }

    public static class Test
    {
        public static async Task SimpleAsync()
        {
            var scriptOptions = ScriptOptions.Default
                .AddImports("System.Xml", "System")
                .WithReferences(Assembly.LoadFrom("Xu.dll"));

            string scp = "using System; using Xu;";

            int iterationCount = 20;

            var script = CSharpScript.Create<int>(scp + "Period pf = new Period(DateTime.Now.AddDays(-Number), DateTime.Now); " + "Console.WriteLine(pf.ToString()); ", scriptOptions, globalsType: typeof(ScriptHost));
            script.Compile();
            var runner = script.CreateDelegate();

            //Compilation gives access to the full set of Roslyn APIs.
            Compilation compilation = script.GetCompilation();

            /*
            foreach (var variable in state.Variables)
                Console.WriteLine($"{variable.Name} = {variable.Value} of type {variable.Type}");
                */
            DateTime startTime = DateTime.Now;
            try
            {
               // int result = await CSharpScript.EvaluateAsync<int>("1 + 2");

               // Console.WriteLine(result);
                //SampleTest pf = new SampleTest(12, 56); Console.WriteLine(pf.ToString());
                //Period pf = new Period(DateTime.Now.AddDays(-1), DateTime.Now);
                //Console.WriteLine(pf.ToString());


                for(int i = 0; i < iterationCount; i++) 
                {
                    //await CSharpScript.EvaluateAsync(scp + "Period pf = new Period(DateTime.Now.AddDays(-Number), DateTime.Now); " + "Console.WriteLine(pf.ToString()); ", scriptOptions, new ScriptHost { Number = i }); // Assembly.GetExecutingAssembly()
                    var state = await script.RunAsync(new ScriptHost { Number = i });

                    //foreach (var variable in state.Variables)
                        //Console.WriteLine($"{variable.Name} = {variable.Value} of type {variable.Type}");
                    //await runner(new ScriptHost { Number = i });
                }


                //await CSharpScript.EvaluateAsync("using System; Console.WriteLine(\"Hello world!\");");
                //await CSharpScript.EvaluateAsync(scp + "Period pf = new Period(DateTime.Now.AddDays(-1), DateTime.Now); " + "Console.WriteLine(pf.ToString()); ", ScriptOptions.Default.WithReferences(typeof(Period).Assembly));
                //await CSharpScript.EvaluateAsync(scp + "Period pf = new Period(DateTime.Now.AddDays(-1), DateTime.Now); " + "Console.WriteLine(pf.ToString()); ", ScriptOptions.Default.WithImports("Xu"));

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

            TimeSpan sp = DateTime.Now - startTime;

            Console.WriteLine("Average execution time is " + sp.TotalSeconds / iterationCount);
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
        static void Main2s(string[] args)
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
