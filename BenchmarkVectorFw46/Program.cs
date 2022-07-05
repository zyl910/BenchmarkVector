using BenchmarkVector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkVectorFw46 {
    class Program {
        static void Main(string[] args) {
            string indent = "";
            TextWriter tw = Console.Out;
            tw.WriteLine("BenchmarkVectorFw46");
            tw.WriteLine();
            BenchmarkVectorDemo.OutputEnvironment(tw, indent);
            tw.WriteLine(string.Format("Main-Vector4.Assembly.CodeBase:\t{0}", typeof(Vector4).Assembly.CodeBase));
            tw.WriteLine(indent);
            BenchmarkVectorDemo.Benchmark(tw, indent);
        }
    }
}
