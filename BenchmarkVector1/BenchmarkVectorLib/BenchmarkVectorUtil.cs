using BenchmarkVector;
using System;
using System.IO;

namespace BenchmarkVectorLib {
    /// <summary>
    /// Benchmark Vector Util
    /// </summary>
    public static class BenchmarkVectorUtil {

        /// <summary>
        /// Output Environment.
        /// </summary>
        /// <param name="tw">Output <see cref="TextWriter"/>.</param>
        /// <param name="indent">The indent.</param>
        public static void OutputEnvironment(TextWriter tw, string indent) {
            BenchmarkVectorDemo.OutputEnvironment(tw, indent);
        }

        /// <summary>
        /// Do Benchmark.
        /// </summary>
        /// <param name="tw">Output <see cref="TextWriter"/>.</param>
        /// <param name="indent">The indent.</param>
        public static void Benchmark(TextWriter tw, string indent) {
            BenchmarkVectorDemo.Benchmark(tw, indent);
        }
    }
}
