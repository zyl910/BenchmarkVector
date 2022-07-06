#if NETCOREAPP3_0_OR_GREATER
#define Allow_Intrinsics
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
#if Allow_Intrinsics
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif

namespace BenchmarkVector {
    /// <summary>
    /// Benchmark Vector Demo
    /// </summary>
    static class BenchmarkVectorDemo {
        /// <summary>
        /// Enabled loop unrolling.
        /// </summary>
        public static bool EnabledLoopUnrolling = true;

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
            tw.WriteLine(string.Format("Vector4.Assembly:\t{0}", assembly));
            tw.WriteLine(string.Format("Vector4.Assembly.CodeBase:\t{0}", assembly.CodeBase));
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
            // SumBaseU4.
            if (EnabledLoopUnrolling) {
                tickBegin = Environment.TickCount;
                rt = SumBaseU4(src, count, loops);
                msUsed = Environment.TickCount - tickBegin;
                mFlops = countMFlops * 1000 / msUsed;
                scale = mFlops / mFlopsBase;
                tw.WriteLine(indent + string.Format("SumBaseU4:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
            }
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
            // SumVectorAvx.
#if Allow_Intrinsics
            if (Avx.IsSupported) {
                try {
                    tickBegin = Environment.TickCount;
                    rt = SumVectorAvx(src, count, loops);
                    msUsed = Environment.TickCount - tickBegin;
                    mFlops = countMFlops * 1000 / msUsed;
                    scale = mFlops / mFlopsBase;
                    tw.WriteLine(indent + string.Format("SumVectorAvx:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
                    // SumVectorAvxSpan.
                    tickBegin = Environment.TickCount;
                    rt = SumVectorAvxSpan(src, count, loops);
                    msUsed = Environment.TickCount - tickBegin;
                    mFlops = countMFlops * 1000 / msUsed;
                    scale = mFlops / mFlopsBase;
                    tw.WriteLine(indent + string.Format("SumVectorAvxSpan:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
                } catch (Exception ex) {
                    tw.WriteLine("Run SumVectorAvx fail!");
                    tw.WriteLine(ex);
                }
            }
#endif
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
        /// Sum - base - Loop unrolling *4.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumBaseU4(float[] src, int count, int loops) {
            float rt = 0; // Result.
            float rt1 = 0;
            float rt2 = 0;
            float rt3 = 0;
            int nBlockWidth = 4; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            int idx; // Index for src data.
            int i;
            for (int j = 0; j < loops; ++j) {
                idx = 0;
                // Block processs.
                for (i = 0; i < cntBlock; ++i) {
                    rt += src[idx];
                    rt1 += src[idx + 1];
                    rt2 += src[idx + 2];
                    rt3 += src[idx + 3];
                    idx += nBlockWidth;
                }
                // Remainder processs.
                for (i = 0; i < cntRem; ++i) {
                    rt += src[idx + i];
                }
            }
            // Reduce.
            rt = rt + rt1 + rt2 + rt3;
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
            if (0 != count % VectorWidth) throw new ArgumentException("The count can't div 4.", "count");
            int vcount = count / VectorWidth;
            Vector4[] vsrc = new Vector4[vcount];
            int p = 0;
            for(int i=0; i< vcount; ++i) {
                vsrc[i] = new Vector4(src[p], src[p + 1], src[p + 2], src[p + 3]);
                p += VectorWidth;
            }
            // body.
            Vector4 vrt = Vector4.Zero;
            for (int j = 0; j < loops; ++j) {
                for (int i = 0; i < vcount; ++i) {
                    vrt += vsrc[i];
                }
            }
            // reduce.
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
            int VectorWidth = Vector<float>.Count;
            if (0 != count % VectorWidth) throw new ArgumentException(string.Format("The count can't div {0}.", VectorWidth), "count");
            int vcount = count / VectorWidth;
            float[] dst = new float[VectorWidth];
            Vector<float>[] vsrc = new Vector<float>[vcount];
            int p = 0;
            for (int i = 0; i < vcount; ++i) {
                vsrc[i] = new Vector<float>(src, p);
                p += VectorWidth;
            }
            // body.
            Vector<float> vrt = Vector<float>.Zero;
            for (int j = 0; j < loops; ++j) {
                for (int i = 0; i < vcount; ++i) {
                    vrt += vsrc[i];
                }
            }
            // reduce.
            vrt.CopyTo(dst);
            for (int i = 0; i < dst.Length; ++i) {
                rt += dst[i];
            }
            return rt;
        }

        /// <summary>
        /// Sum - Vector AVX.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorAvx(float[] src, int count, int loops) {
#if Allow_Intrinsics
            float rt = 0; // Result.
            //int VectorWidth = 32 / 4; // sizeof(__m256) / sizeof(float); // Block width.
            int VectorWidth = Vector256<float>.Count;
            int nBlockWidth = VectorWidth; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector256<float> vrt = Vector256<float>.Zero; // Vector result.
            int p; // Index for src data.
            int i;
            // Load.
            Vector256<float>[] vsrc = new Vector256<float>[cntBlock];
            p = 0;
            for (i = 0; i < cntBlock; ++i) {
                vsrc[i] = Vector256.Create(src[p], src[p + 1], src[p + 2], src[p + 3], src[p + 4], src[p + 5], src[p + 6], src[p + 7]); // Load. vsrc[i] = *p;
                p += VectorWidth;
            }
            // Body.
            for (int j = 0; j < loops; ++j) {
                p = 0;
                // Vector processs.
                for (i = 0; i < cntBlock; ++i) {
                    vrt = Avx.Add(vrt, vsrc[i]);    // Add. vrt += vsrc[i];
                    p += nBlockWidth;
                }
                // Remainder processs.
                for (i = 0; i < cntRem; ++i) {
                    rt += src[p + i];
                }
            }
            // Reduce.
            for (i = 0; i < VectorWidth; ++i) {
                rt += vrt.GetElement(i);
            }
            return rt;
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// Sum - Vector AVX - Span.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorAvxSpan(float[] src, int count, int loops) {
#if Allow_Intrinsics
            float rt = 0; // Result.
            //int VectorWidth = 32 / 4; // sizeof(__m256) / sizeof(float); // Block width.
            int VectorWidth = Vector256<float>.Count;
            int nBlockWidth = VectorWidth; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector256<float> vrt = Vector256<float>.Zero; // Vector result.
            int p; // Index for src data.
            ReadOnlySpan<Vector256<float>> pV; // Span for Vector.
            int i;
            //// Load.
            //Vector256<float>[] vsrc = new Vector256<float>[cntBlock];
            //p = 0;
            //for (i = 0; i < cntBlock; ++i) {
            //    vsrc[i] = Vector256.Create(src[p], src[p + 1], src[p + 2], src[p + 3], src[p + 4], src[p + 5], src[p + 6], src[p + 7]); // Load. vsrc[i] = *p;
            //    p += VectorWidth;
            //}
            // Body.
            for (int j = 0; j < loops; ++j) {
                p = 0;
                // Vector processs.
                pV = System.Runtime.InteropServices.MemoryMarshal.Cast<float, Vector256<float> >(new Span<float>(src)); // Reinterpret cast. `float*` to `Vector256<float>`.
                for (i = 0; i < cntBlock; ++i) {
                    vrt = Avx.Add(vrt, pV[i]);    // Add. vrt += vsrc[i];
                    p += nBlockWidth;
                }
                // Remainder processs.
                for (i = 0; i < cntRem; ++i) {
                    rt += src[p + i];
                }
            }
            // Reduce.
            for (i = 0; i < VectorWidth; ++i) {
                rt += vrt.GetElement(i);
            }
            return rt;
#else
            throw new NotSupportedException();
#endif
        }
    }
}
