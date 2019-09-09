using ReqTools;
using System;
using System.Threading.Tasks;

namespace ConstoleTester
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var reqParser = new ReqParser();
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            await reqParser.ParseToFileAsync(new Progress<string>(x => { Console.WriteLine($"Stan:{x}"); }), @"C:\Users\KMIM\source\repos\ReqComparer\VisualComparer\bin\Release\_Fitting_SW_PR_Phoenix.htm");
            timer.Stop();
            Console.WriteLine($"Czas parsowania: {timer.Elapsed}");

            timer.Restart();
            await reqParser.GetReqsFromCachedFile();
            timer.Stop();
            Console.WriteLine($"Czas ładowania: {timer.Elapsed}");
            //Console.ReadKey(true);
        }
    }
}
