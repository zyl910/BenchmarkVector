using BenchmarkVectorLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkVectorFw46UseLib {
    class Program {
        static void Main(string[] args) {
            string indent = "";
            TextWriter tw = Console.Out;
            tw.WriteLine("BenchmarkVectorFw46UseLib");
            tw.WriteLine();
            BenchmarkVectorUtil.OutputEnvironment(tw, indent);
            tw.WriteLine(indent);
            BenchmarkVectorUtil.Benchmark(tw, indent);
        }
    }
}
