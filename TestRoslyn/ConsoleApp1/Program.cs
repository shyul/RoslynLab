using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xu;

namespace TestRoslyn
{
    class Program
    {
        static void Main(string[] args)
        {


            _ = Test.SimpleAsync();


            Console.WriteLine("\n\nJob Done, Press any key to exit.");
            //Console.ReadLine();
            Console.ReadKey();
            //while (true) ;
        }
    }
}
