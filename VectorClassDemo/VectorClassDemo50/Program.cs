using System;
using System.IO;
using VectorClassDemo;

namespace VectorClassDemo50 {
    class Program {
        static void Main(string[] args) {
            string indent = "";
            TextWriter tw = Console.Out;
            tw.WriteLine("VectorClassDemo50");
            tw.WriteLine();
            VectorDemo.OutputEnvironment(tw, indent);
            tw.WriteLine();
            VectorDemo.Run(tw, indent);
        }
    }
}
