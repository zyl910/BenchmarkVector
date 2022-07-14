using System;
using System.IO;
using VectorClassDemo;

namespace VectorClassDemo20 {
    class Program {
        static void Main(string[] args) {
            string indent = "";
            TextWriter tw = Console.Out;
            tw.WriteLine("VectorClassDemo20");
            tw.WriteLine();
            VectorDemo.OutputEnvironment(tw, indent);
            tw.WriteLine();
            VectorDemo.Run(tw, indent);
        }
    }
}
