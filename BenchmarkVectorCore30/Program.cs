using BenchmarkVector;
using System;
using System.IO;

namespace BenchmarkVectorCore30 {
    class Program {
        static void Main(string[] args) {
            string indent = "";
            TextWriter tw = Console.Out;
            tw.WriteLine("BenchmarkVectorCore30");
            tw.WriteLine();
            BenchmarkVectorDemo.OutputEnvironment(tw, indent);
            tw.WriteLine(string.Format("Main-Vector4.Assembly.CodeBase:\t{0}", typeof(System.Numerics.Vector4).Assembly.CodeBase));
            tw.WriteLine(indent);
            BenchmarkVectorDemo.Benchmark(tw, indent);
        }
    }
}
