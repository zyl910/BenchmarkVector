using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
#if Allow_Intrinsics
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif
using System.Runtime.CompilerServices;

namespace BenchmarkVector {
    /// <summary>
    /// Benchmark Vector Demo
    /// </summary>
    static class BenchmarkVectorDemo {
        /// <summary>
        /// Is release make.
        /// </summary>
        public static readonly bool IsRelease =
#if DEBUG
            false
#else
            true
#endif
        ;

        /// <summary>
        /// Output Environment.
        /// </summary>
        /// <param name="tw">Output <see cref="TextWriter"/>.</param>
        /// <param name="indent">The indent.</param>
        public static void OutputEnvironment(TextWriter tw, string indent) {
            if (null == tw) return;
            if (null == indent) indent="";
            //string indentNext = indent + "\t";
            tw.WriteLine(indent + string.Format("IsRelease:\t{0}", IsRelease));
            tw.WriteLine(indent + string.Format("EnvironmentVariable(PROCESSOR_IDENTIFIER):\t{0}", Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")));
            tw.WriteLine(indent + string.Format("Environment.ProcessorCount:\t{0}", Environment.ProcessorCount));
            tw.WriteLine(indent + string.Format("Environment.Is64BitOperatingSystem:\t{0}", Environment.Is64BitOperatingSystem));
            tw.WriteLine(indent + string.Format("Environment.Is64BitProcess:\t{0}", Environment.Is64BitProcess));
            tw.WriteLine(indent + string.Format("Environment.OSVersion:\t{0}", Environment.OSVersion));
            tw.WriteLine(indent + string.Format("Environment.Version:\t{0}", Environment.Version));
            //tw.WriteLine(indent + string.Format("RuntimeEnvironment.GetSystemVersion:\t{0}", System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion())); // Same Environment.Version
            tw.WriteLine(indent + string.Format("RuntimeEnvironment.GetRuntimeDirectory:\t{0}", System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()));
#if (NET47 || NET462 || NET461 || NET46 || NET452 || NET451 || NET45 || NET40 || NET35 || NET20) || (NETSTANDARD1_0)
#else
            tw.WriteLine(indent + string.Format("RuntimeInformation.FrameworkDescription:\t{0}", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription));
#endif
            tw.WriteLine(indent + string.Format("BitConverter.IsLittleEndian:\t{0}", BitConverter.IsLittleEndian));
            tw.WriteLine(indent + string.Format("IntPtr.Size:\t{0}", IntPtr.Size));
            tw.WriteLine(indent + string.Format("Vector.IsHardwareAccelerated:\t{0}", Vector.IsHardwareAccelerated));
            tw.WriteLine(indent + string.Format("Vector<byte>.Count:\t{0}\t# {1}bit", Vector<byte>.Count, Vector<byte>.Count * sizeof(byte) * 8));
            tw.WriteLine(indent + string.Format("Vector<float>.Count:\t{0}\t# {1}bit", Vector<float>.Count, Vector<float>.Count*sizeof(float)*8));
            tw.WriteLine(indent + string.Format("Vector<double>.Count:\t{0}\t# {1}bit", Vector<double>.Count, Vector<double>.Count * sizeof(double) * 8));
            Assembly assembly = typeof(Vector4).GetTypeInfo().Assembly;
            //tw.WriteLine(string.Format("Vector4.Assembly:\t{0}", assembly));
            tw.WriteLine(string.Format("Vector4.Assembly.CodeBase:\t{0}", assembly.CodeBase));
            assembly = typeof(Vector<float>).GetTypeInfo().Assembly;
            tw.WriteLine(string.Format("Vector<T>.Assembly.CodeBase:\t{0}", assembly.CodeBase));
        }

        /// <summary>
        /// Do Benchmark.
        /// </summary>
        /// <param name="tw">Output <see cref="TextWriter"/>.</param>
        /// <param name="indent">The indent.</param>
        public static void Benchmark(TextWriter tw, string indent) {
            if (null == tw) return;
            if (null == indent) indent = "";
            //string indentNext = indent + "\t";
            // init.
            int tickBegin, msUsed;
            double mFlops; // MFLOPS/s .
            double scale;
            float rt;
            const int count = 1024*4;
            const int loops = 1000 * 1000;
            //const int loops = 1;
            const double countMFlops = count * (double)loops / (1000.0 * 1000);
            float[] src = new float[count];
            for(int i=0; i< count; ++i) {
                src[i] = i;
            }
            tw.WriteLine(indent + string.Format("Benchmark: \tcount={0}, loops={1}, countMFlops={2}", count, loops, countMFlops));
            // SumBase.
            tickBegin = Environment.TickCount;
            rt = SumBase(src, count, loops);
            msUsed = Environment.TickCount - tickBegin;
            mFlops = countMFlops * 1000 / msUsed;
            tw.WriteLine(indent + string.Format("SumBase:\t{0}\t# msUsed={1}, MFLOPS/s={2}", rt, msUsed, mFlops));
            double mFlopsBase = mFlops;
            // SumVector4.
            tickBegin = Environment.TickCount;
            rt = SumVector4(src, count, loops);
            msUsed = Environment.TickCount - tickBegin;
            mFlops = countMFlops * 1000 / msUsed;
            scale = mFlops / mFlopsBase;
            tw.WriteLine(indent + string.Format("SumVector4:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
            // SumVectorT.
            tickBegin = Environment.TickCount;
            rt = SumVectorT(src, count, loops);
            msUsed = Environment.TickCount - tickBegin;
            mFlops = countMFlops * 1000 / msUsed;
            scale = mFlops / mFlopsBase;
            tw.WriteLine(indent + string.Format("SumVectorT:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
        }

        /// <summary>
        /// Sum - base.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumBase(float[] src, int count, int loops) {
            float rt = 0; // Result.
            for (int j=0; j< loops; ++j) {
                for(int i=0; i< count; ++i) {
                    rt += src[i];
                }
            }
            return rt;
        }

        /// <summary>
        /// Sum - Vector4.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVector4(float[] src, int count, int loops) {
            float rt = 0; // Result.
            const int VectorWidth = 4;
            int nBlockWidth = VectorWidth; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector4 vrt = Vector4.Zero; // Vector result.
            int p; // Index for src data.
            int i;
            // Load.
            Vector4[] vsrc = new Vector4[cntBlock]; // Vector src.
            p = 0;
            for (i = 0; i < vsrc.Length; ++i) {
                vsrc[i] = new Vector4(src[p], src[p + 1], src[p + 2], src[p + 3]);
                p += VectorWidth;
            }
            // Body.
            for (int j = 0; j < loops; ++j) {
                // Vector processs.
                for (i = 0; i < cntBlock; ++i) {
                    vrt += vsrc[i]; // Add.
                }
                // Remainder processs.
                p = cntBlock * nBlockWidth;
                for (i = 0; i < cntRem; ++i) {
                    rt += src[p + i];
                }
            }
            // Reduce.
            rt = vrt.X + vrt.Y + vrt.Z + vrt.W;
            return rt;
        }

        /// <summary>
        /// Sum - Vector<T>.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorT(float[] src, int count, int loops) {
            float rt = 0; // Result.
            int VectorWidth = Vector<float>.Count; // Block width.
            int nBlockWidth = VectorWidth; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector<float> vrt = Vector<float>.Zero; // Vector result.
            int p; // Index for src data.
            int i;
            // Load.
            Vector<float>[] vsrc = new Vector<float>[cntBlock]; // Vector src.
            p = 0;
            for (i = 0; i < vsrc.Length; ++i) {
                vsrc[i] = new Vector<float>(src, p);
                p += VectorWidth;
            }
            // Body.
            for (int j = 0; j < loops; ++j) {
                // Vector processs.
                for (i = 0; i < cntBlock; ++i) {
                    vrt += vsrc[i]; // Add.
                }
                // Remainder processs.
                p = cntBlock * nBlockWidth;
                for (i = 0; i < cntRem; ++i) {
                    rt += src[p + i];
                }
            }
            // Reduce.
            for (i = 0; i < VectorWidth; ++i) {
                rt += vrt[i];
            }
            return rt;
        }

    }
}
