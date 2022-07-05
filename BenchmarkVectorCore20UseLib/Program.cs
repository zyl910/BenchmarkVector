using BenchmarkVectorLib;
using System;
using System.IO;
using System.Numerics;

namespace BenchmarkVectorCore20UseLib {
    class Program {
        static void Main(string[] args) {
            string indent = "";
            TextWriter tw = Console.Out;
            tw.WriteLine("BenchmarkVectorCore20UseLib");
            tw.WriteLine();
            BenchmarkVectorUtil.OutputEnvironment(tw, indent);
            tw.WriteLine(string.Format("Main-Vector4.Assembly.CodeBase:\t{0}", typeof(Vector4).Assembly.CodeBase));
            tw.WriteLine(indent);
            BenchmarkVectorUtil.Benchmark(tw, indent);
        }
    }
}
