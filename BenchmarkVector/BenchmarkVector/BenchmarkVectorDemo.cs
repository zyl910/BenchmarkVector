#if NETCOREAPP3_0_OR_GREATER
#define Allow_Intrinsics
#endif

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
            // SumVector4U4.
            if (EnabledLoopUnrolling) {
                tickBegin = Environment.TickCount;
                rt = SumVector4U4(src, count, loops);
                msUsed = Environment.TickCount - tickBegin;
                mFlops = countMFlops * 1000 / msUsed;
                scale = mFlops / mFlopsBase;
                tw.WriteLine(indent + string.Format("SumVector4U4:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
            }
            // SumVectorT.
            tickBegin = Environment.TickCount;
            rt = SumVectorT(src, count, loops);
            msUsed = Environment.TickCount - tickBegin;
            mFlops = countMFlops * 1000 / msUsed;
            scale = mFlops / mFlopsBase;
            tw.WriteLine(indent + string.Format("SumVectorT:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
            // SumVectorTU4.
            if (EnabledLoopUnrolling) {
                tickBegin = Environment.TickCount;
                rt = SumVectorTU4(src, count, loops);
                msUsed = Environment.TickCount - tickBegin;
                mFlops = countMFlops * 1000 / msUsed;
                scale = mFlops / mFlopsBase;
                tw.WriteLine(indent + string.Format("SumVectorTU4:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
            }
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
                    // SumVectorAvxPtr.
                    tickBegin = Environment.TickCount;
                    rt = SumVectorAvxPtr(src, count, loops);
                    msUsed = Environment.TickCount - tickBegin;
                    mFlops = countMFlops * 1000 / msUsed;
                    scale = mFlops / mFlopsBase;
                    tw.WriteLine(indent + string.Format("SumVectorAvxPtr:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
                    // SumVectorAvxU4.
                    if (EnabledLoopUnrolling) {
                        tickBegin = Environment.TickCount;
                        rt = SumVectorAvxU4(src, count, loops);
                        msUsed = Environment.TickCount - tickBegin;
                        mFlops = countMFlops * 1000 / msUsed;
                        scale = mFlops / mFlopsBase;
                        tw.WriteLine(indent + string.Format("SumVectorAvxU4:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
                        // SumVectorAvxSpanU4.
                        tickBegin = Environment.TickCount;
                        rt = SumVectorAvxSpanU4(src, count, loops);
                        msUsed = Environment.TickCount - tickBegin;
                        mFlops = countMFlops * 1000 / msUsed;
                        scale = mFlops / mFlopsBase;
                        tw.WriteLine(indent + string.Format("SumVectorAvxSpanU4:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
                        // SumVectorAvxPtrU4.
                        tickBegin = Environment.TickCount;
                        rt = SumVectorAvxPtrU4(src, count, loops);
                        msUsed = Environment.TickCount - tickBegin;
                        mFlops = countMFlops * 1000 / msUsed;
                        scale = mFlops / mFlopsBase;
                        tw.WriteLine(indent + string.Format("SumVectorAvxPtrU4:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
                        // SumVectorAvxPtrU16.
                        tickBegin = Environment.TickCount;
                        rt = SumVectorAvxPtrU16(src, count, loops);
                        msUsed = Environment.TickCount - tickBegin;
                        mFlops = countMFlops * 1000 / msUsed;
                        scale = mFlops / mFlopsBase;
                        tw.WriteLine(indent + string.Format("SumVectorAvxPtrU16:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
                        // SumVectorAvxPtrU16A.
                        tickBegin = Environment.TickCount;
                        rt = SumVectorAvxPtrU16A(src, count, loops);
                        msUsed = Environment.TickCount - tickBegin;
                        mFlops = countMFlops * 1000 / msUsed;
                        scale = mFlops / mFlopsBase;
                        tw.WriteLine(indent + string.Format("SumVectorAvxPtrU16A:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale));
                        // SumVectorAvxPtrUX.
                        int[] LoopUnrollingArray = { 4, 8, 16 };
                        foreach (int loopUnrolling in LoopUnrollingArray) {
                            tickBegin = Environment.TickCount;
                            rt = SumVectorAvxPtrUX(src, count, loops, loopUnrolling);
                            msUsed = Environment.TickCount - tickBegin;
                            mFlops = countMFlops * 1000 / msUsed;
                            scale = mFlops / mFlopsBase;
                            tw.WriteLine(indent + string.Format("SumVectorAvxPtrUX[{4}]:\t{0}\t# msUsed={1}, MFLOPS/s={2}, scale={3}", rt, msUsed, mFlops, scale, loopUnrolling));
                        }
                    }
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
            int p; // Index for src data.
            int i;
            for (int j = 0; j < loops; ++j) {
                p = 0;
                // Block processs.
                for (i = 0; i < cntBlock; ++i) {
                    rt += src[p];
                    rt1 += src[p + 1];
                    rt2 += src[p + 2];
                    rt3 += src[p + 3];
                    p += nBlockWidth;
                }
                // Remainder processs.
                //p = cntBlock * nBlockWidth;
                for (i = 0; i < cntRem; ++i) {
                    rt += src[p + i];
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
        /// Sum - Vector4 - Loop unrolling *4.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVector4U4(float[] src, int count, int loops) {
            float rt = 0; // Result.
            const int LoopUnrolling = 4;
            const int VectorWidth = 4;
            int nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector4 vrt = Vector4.Zero; // Vector result.
            Vector4 vrt1 = Vector4.Zero;
            Vector4 vrt2 = Vector4.Zero;
            Vector4 vrt3 = Vector4.Zero;
            int p; // Index for src data.
            int i;
            // Load.
            Vector4[] vsrc = new Vector4[count / VectorWidth]; // Vector src.
            p = 0;
            for (i = 0; i < vsrc.Length; ++i) {
                vsrc[i] = new Vector4(src[p], src[p + 1], src[p + 2], src[p + 3]);
                p += VectorWidth;
            }
            // Body.
            for (int j = 0; j < loops; ++j) {
                p = 0;
                // Vector processs.
                for (i = 0; i < cntBlock; ++i) {
                    vrt += vsrc[p]; // Add.
                    vrt1 += vsrc[p + 1];
                    vrt2 += vsrc[p + 2];
                    vrt3 += vsrc[p + 3];
                    p += LoopUnrolling;
                }
                // Remainder processs.
                p = cntBlock * nBlockWidth;
                for (i = 0; i < cntRem; ++i) {
                    rt += src[p + i];
                }
            }
            // Reduce.
            vrt = vrt + vrt1 + vrt2 + vrt3;
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

        /// <summary>
        /// Sum - Vector<T> - Loop unrolling *4.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorTU4(float[] src, int count, int loops) {
            float rt = 0; // Result.
            const int LoopUnrolling = 4;
            int VectorWidth = Vector<float>.Count; // Block width.
            int nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector<float> vrt = Vector<float>.Zero; // Vector result.
            Vector<float> vrt1 = Vector<float>.Zero;
            Vector<float> vrt2 = Vector<float>.Zero;
            Vector<float> vrt3 = Vector<float>.Zero;
            int p; // Index for src data.
            int i;
            // Load.
            Vector<float>[] vsrc = new Vector<float>[count / VectorWidth]; // Vector src.
            p = 0;
            for (i = 0; i < vsrc.Length; ++i) {
                vsrc[i] = new Vector<float>(src, p);
                p += VectorWidth;
            }
            // Body.
            for (int j = 0; j < loops; ++j) {
                p = 0;
                // Vector processs.
                for (i = 0; i < cntBlock; ++i) {
                    vrt += vsrc[p]; // Add.
                    vrt1 += vsrc[p + 1];
                    vrt2 += vsrc[p + 1];
                    vrt3 += vsrc[p + 1];
                    p += LoopUnrolling;
                }
                // Remainder processs.
                p = cntBlock * nBlockWidth;
                for (i = 0; i < cntRem; ++i) {
                    rt += src[p + i];
                }
            }
            // Reduce.
            vrt = vrt + vrt1 + vrt2 + vrt3;
            for (i = 0; i < VectorWidth; ++i) {
                rt += vrt[i];
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
            //int VectorWidth = 32 / 4; // sizeof(__m256) / sizeof(float);
            int VectorWidth = Vector256<float>.Count; // Block width.
            int nBlockWidth = VectorWidth; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector256<float> vrt = Vector256<float>.Zero; // Vector result.
            int p; // Index for src data.
            int i;
            // Load.
            Vector256<float>[] vsrc = new Vector256<float>[cntBlock]; // Vector src.
            p = 0;
            for (i = 0; i < cntBlock; ++i) {
                vsrc[i] = Vector256.Create(src[p], src[p + 1], src[p + 2], src[p + 3], src[p + 4], src[p + 5], src[p + 6], src[p + 7]); // Load.
                p += VectorWidth;
            }
            // Body.
            for (int j = 0; j < loops; ++j) {
                // Vector processs.
                for (i = 0; i < cntBlock; ++i) {
                    vrt = Avx.Add(vrt, vsrc[i]);    // Add. vrt += vsrc[i];
                }
                // Remainder processs.
                p = cntBlock * nBlockWidth;
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
        /// Sum - Vector AVX - Loop unrolling *4.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorAvxU4(float[] src, int count, int loops) {
#if Allow_Intrinsics
            float rt = 0; // Result.
            const int LoopUnrolling = 4;
            int VectorWidth = Vector256<float>.Count; // Block width.
            int nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector256<float> vrt = Vector256<float>.Zero; // Vector result.
            Vector256<float> vrt1 = Vector256<float>.Zero;
            Vector256<float> vrt2 = Vector256<float>.Zero;
            Vector256<float> vrt3 = Vector256<float>.Zero;
            int p; // Index for src data.
            int i;
            // Load.
            Vector256<float>[] vsrc = new Vector256<float>[count / VectorWidth]; // Vector src.
            p = 0;
            for (i = 0; i < vsrc.Length; ++i) {
                vsrc[i] = Vector256.Create(src[p], src[p + 1], src[p + 2], src[p + 3], src[p + 4], src[p + 5], src[p + 6], src[p + 7]); // Load.
                p += VectorWidth;
            }
            // Body.
            for (int j = 0; j < loops; ++j) {
                p = 0;
                // Vector processs.
                for (i = 0; i < cntBlock; ++i) {
                    vrt = Avx.Add(vrt, vsrc[p]);    // Add. vrt += vsrc[p];
                    vrt1 = Avx.Add(vrt1, vsrc[p + 1]);
                    vrt2 = Avx.Add(vrt2, vsrc[p + 2]);
                    vrt3 = Avx.Add(vrt3, vsrc[p + 3]);
                    p += LoopUnrolling;
                }
                // Remainder processs.
                p = cntBlock * nBlockWidth;
                for (i = 0; i < cntRem; ++i) {
                    rt += src[p + i];
                }
            }
            // Reduce.
            vrt = Avx.Add(Avx.Add(vrt, vrt1), Avx.Add(vrt2, vrt3)); // vrt = vrt + vrt1 + vrt2 + vrt3;
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
            int VectorWidth = Vector256<float>.Count; // Block width.
            int nBlockWidth = VectorWidth; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector256<float> vrt = Vector256<float>.Zero; // Vector result.
            int p; // Index for src data.
            ReadOnlySpan<Vector256<float>> vsrc; // Vector src.
            int i;
            // Body.
            for (int j = 0; j < loops; ++j) {
                // Vector processs.
                vsrc = System.Runtime.InteropServices.MemoryMarshal.Cast<float, Vector256<float> >(new Span<float>(src)); // Reinterpret cast. `float*` to `Vector256<float>*`.
                for (i = 0; i < cntBlock; ++i) {
                    vrt = Avx.Add(vrt, vsrc[i]);    // Add. vrt += vsrc[i];
                }
                // Remainder processs.
                p = cntBlock * nBlockWidth;
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
        /// Sum - Vector AVX - Span - Loop unrolling *4.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorAvxSpanU4(float[] src, int count, int loops) {
#if Allow_Intrinsics
            float rt = 0; // Result.
            const int LoopUnrolling = 4;
            int VectorWidth = Vector256<float>.Count; // Block width.
            int nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
            int cntBlock = count / nBlockWidth; // Block count.
            int cntRem = count % nBlockWidth; // Remainder count.
            Vector256<float> vrt = Vector256<float>.Zero; // Vector result.
            Vector256<float> vrt1 = Vector256<float>.Zero;
            Vector256<float> vrt2 = Vector256<float>.Zero;
            Vector256<float> vrt3 = Vector256<float>.Zero;
            int p; // Index for src data.
            ReadOnlySpan<Vector256<float>> vsrc; // Vector src.
            int i;
            // Body.
            for (int j = 0; j < loops; ++j) {
                p = 0;
                // Vector processs.
                vsrc = System.Runtime.InteropServices.MemoryMarshal.Cast<float, Vector256<float>>(new Span<float>(src)); // Reinterpret cast. `float*` to `Vector256<float>*`.
                for (i = 0; i < cntBlock; ++i) {
                    vrt = Avx.Add(vrt, vsrc[p]);    // Add. vrt += vsrc[p];
                    vrt1 = Avx.Add(vrt1, vsrc[p + 1]);
                    vrt2 = Avx.Add(vrt2, vsrc[p + 2]);
                    vrt3 = Avx.Add(vrt3, vsrc[p + 3]);
                    p += LoopUnrolling;
                }
                // Remainder processs.
                p = cntBlock * nBlockWidth;
                for (i = 0; i < cntRem; ++i) {
                    rt += src[p + i];
                }
            }
            // Reduce.
            vrt = Avx.Add(Avx.Add(vrt, vrt1), Avx.Add(vrt2, vrt3)); // vrt = vrt + vrt1 + vrt2 + vrt3;
            for (i = 0; i < VectorWidth; ++i) {
                rt += vrt.GetElement(i);
            }
            return rt;
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// Sum - Vector AVX - Ptr.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorAvxPtr(float[] src, int count, int loops) {
#if Allow_Intrinsics && UNSAFE
            unsafe {
                float rt = 0; // Result.
                int VectorWidth = Vector256<float>.Count; // Block width.
                int nBlockWidth = VectorWidth; // Block width.
                int cntBlock = count / nBlockWidth; // Block count.
                int cntRem = count % nBlockWidth; // Remainder count.
                Vector256<float> vrt = Vector256<float>.Zero; // Vector result.
                Vector256<float> vload;
                float* p; // Pointer for src data.
                int i;
                // Body.
                fixed(float* p0 = &src[0]) {
                    for (int j = 0; j < loops; ++j) {
                        p = p0;
                        // Vector processs.
                        for (i = 0; i < cntBlock; ++i) {
                            vload = Avx.LoadVector256(p);    // Load. vload = *(*__m256)p;
                            vrt = Avx.Add(vrt, vload);    // Add. vrt += vsrc[i];
                            p += nBlockWidth;
                        }
                        // Remainder processs.
                        for (i = 0; i < cntRem; ++i) {
                            rt += p[i];
                        }
                    }
                }
                // Reduce.
                for (i = 0; i < VectorWidth; ++i) {
                    rt += vrt.GetElement(i);
                }
                return rt;
            }
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// Sum - Vector AVX - Ptr - Loop unrolling *4.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorAvxPtrU4(float[] src, int count, int loops) {
#if Allow_Intrinsics && UNSAFE
            unsafe {
                float rt = 0; // Result.
                const int LoopUnrolling = 4;
                int VectorWidth = Vector256<float>.Count; // Block width.
                int nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
                int cntBlock = count / nBlockWidth; // Block count.
                int cntRem = count % nBlockWidth; // Remainder count.
                Vector256<float> vrt = Vector256<float>.Zero; // Vector result.
                Vector256<float> vrt1 = Vector256<float>.Zero;
                Vector256<float> vrt2 = Vector256<float>.Zero;
                Vector256<float> vrt3 = Vector256<float>.Zero;
                Vector256<float> vload;
                Vector256<float> vload1;
                Vector256<float> vload2;
                Vector256<float> vload3;
                float* p; // Pointer for src data.
                int i;
                // Body.
                fixed (float* p0 = &src[0]) {
                    for (int j = 0; j < loops; ++j) {
                        p = p0;
                        // Vector processs.
                        for (i = 0; i < cntBlock; ++i) {
                            vload = Avx.LoadVector256(p);    // Load. vload = *(*__m256)p;
                            vload1 = Avx.LoadVector256(p + VectorWidth * 1);
                            vload2 = Avx.LoadVector256(p + VectorWidth * 2);
                            vload3 = Avx.LoadVector256(p + VectorWidth * 3);
                            vrt = Avx.Add(vrt, vload);    // Add. vrt += vsrc[i];
                            vrt1 = Avx.Add(vrt1, vload1);
                            vrt2 = Avx.Add(vrt2, vload2);
                            vrt3 = Avx.Add(vrt3, vload3);
                            p += nBlockWidth;
                        }
                        // Remainder processs.
                        for (i = 0; i < cntRem; ++i) {
                            rt += p[i];
                        }
                    }
                }
                // Reduce.
                vrt = Avx.Add(Avx.Add(vrt, vrt1), Avx.Add(vrt2, vrt3)); // vrt = vrt + vrt1 + vrt2 + vrt3;
                for (i = 0; i < VectorWidth; ++i) {
                    rt += vrt.GetElement(i);
                }
                return rt;
            }
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// Sum - Vector AVX - Ptr - Loop unrolling *16.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorAvxPtrU16(float[] src, int count, int loops) {
#if Allow_Intrinsics && UNSAFE
            unsafe {
                float rt = 0; // Result.
                const int LoopUnrolling = 16;
                int VectorWidth = Vector256<float>.Count; // Block width.
                int nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
                int cntBlock = count / nBlockWidth; // Block count.
                int cntRem = count % nBlockWidth; // Remainder count.
                Vector256<float> vrt = Vector256<float>.Zero; // Vector result.
                Vector256<float> vrt1 = Vector256<float>.Zero;
                Vector256<float> vrt2 = Vector256<float>.Zero;
                Vector256<float> vrt3 = Vector256<float>.Zero;
                Vector256<float> vrt4 = Vector256<float>.Zero;
                Vector256<float> vrt5 = Vector256<float>.Zero;
                Vector256<float> vrt6 = Vector256<float>.Zero;
                Vector256<float> vrt7 = Vector256<float>.Zero;
                Vector256<float> vrt8 = Vector256<float>.Zero;
                Vector256<float> vrt9 = Vector256<float>.Zero;
                Vector256<float> vrt10 = Vector256<float>.Zero;
                Vector256<float> vrt11 = Vector256<float>.Zero;
                Vector256<float> vrt12 = Vector256<float>.Zero;
                Vector256<float> vrt13 = Vector256<float>.Zero;
                Vector256<float> vrt14 = Vector256<float>.Zero;
                Vector256<float> vrt15 = Vector256<float>.Zero;
                float* p; // Pointer for src data.
                int i;
                // Body.
                fixed (float* p0 = &src[0]) {
                    for (int j = 0; j < loops; ++j) {
                        p = p0;
                        // Vector processs.
                        for (i = 0; i < cntBlock; ++i) {
                            //vload = Avx.LoadVector256(p);    // Load. vload = *(*__m256)p;
                            vrt = Avx.Add(vrt, Avx.LoadVector256(p)); // Add. vrt[k] += *((*__m256)(p)+k);
                            vrt1 = Avx.Add(vrt1, Avx.LoadVector256(p + VectorWidth * 1));
                            vrt2 = Avx.Add(vrt2, Avx.LoadVector256(p + VectorWidth * 2));
                            vrt3 = Avx.Add(vrt3, Avx.LoadVector256(p + VectorWidth * 3));
                            vrt4 = Avx.Add(vrt4, Avx.LoadVector256(p + VectorWidth * 4));
                            vrt5 = Avx.Add(vrt5, Avx.LoadVector256(p + VectorWidth * 5));
                            vrt6 = Avx.Add(vrt6, Avx.LoadVector256(p + VectorWidth * 6));
                            vrt7 = Avx.Add(vrt7, Avx.LoadVector256(p + VectorWidth * 7));
                            vrt8 = Avx.Add(vrt8, Avx.LoadVector256(p + VectorWidth * 8));
                            vrt9 = Avx.Add(vrt9, Avx.LoadVector256(p + VectorWidth * 9));
                            vrt10 = Avx.Add(vrt10, Avx.LoadVector256(p + VectorWidth * 10));
                            vrt11 = Avx.Add(vrt11, Avx.LoadVector256(p + VectorWidth * 11));
                            vrt12 = Avx.Add(vrt12, Avx.LoadVector256(p + VectorWidth * 12));
                            vrt13 = Avx.Add(vrt13, Avx.LoadVector256(p + VectorWidth * 13));
                            vrt14 = Avx.Add(vrt14, Avx.LoadVector256(p + VectorWidth * 14));
                            vrt15 = Avx.Add(vrt15, Avx.LoadVector256(p + VectorWidth * 15));
                            p += nBlockWidth;
                        }
                        // Remainder processs.
                        for (i = 0; i < cntRem; ++i) {
                            rt += p[i];
                        }
                    }
                }
                // Reduce.
                vrt = Avx.Add( Avx.Add( Avx.Add(Avx.Add(vrt, vrt1), Avx.Add(vrt2, vrt3))
                    , Avx.Add(Avx.Add(vrt4, vrt5), Avx.Add(vrt6, vrt7)) )
                    , Avx.Add( Avx.Add(Avx.Add(vrt8, vrt9), Avx.Add(vrt10, vrt11))
                    , Avx.Add(Avx.Add(vrt12, vrt13), Avx.Add(vrt14, vrt15)) ) )
                ; // vrt = vrt + vrt1 + vrt2 + vrt3 + ... vrt15;
                for (i = 0; i < VectorWidth; ++i) {
                    rt += vrt.GetElement(i);
                }
                return rt;
            }
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// Sum - Vector AVX - Ptr - Loop unrolling *16 - Array.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorAvxPtrU16A(float[] src, int count, int loops) {
#if Allow_Intrinsics && UNSAFE
            unsafe {
                float rt = 0; // Result.
                const int LoopUnrolling = 16;
                int VectorWidth = Vector256<float>.Count; // Block width.
                int nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
                int cntBlock = count / nBlockWidth; // Block count.
                int cntRem = count % nBlockWidth; // Remainder count.
                int i;
                //Vector256<float>[] vrt = new Vector256<float>[LoopUnrolling]; // Vector result.
                Vector256<float>* vrt = stackalloc Vector256<float>[LoopUnrolling]; ; // Vector result.
                for (i = 0; i< LoopUnrolling; ++i) {
                    vrt[i] = Vector256<float>.Zero;
                }
                float* p; // Pointer for src data.
                // Body.
                fixed (float* p0 = &src[0]) {
                    for (int j = 0; j < loops; ++j) {
                        p = p0;
                        // Vector processs.
                        for (i = 0; i < cntBlock; ++i) {
                            //vload = Avx.LoadVector256(p);    // Load. vload = *(*__m256)p;
                            vrt[0] = Avx.Add(vrt[0], Avx.LoadVector256(p)); // Add. vrt[k] += *((*__m256)(p)+k);
                            vrt[1] = Avx.Add(vrt[1], Avx.LoadVector256(p + VectorWidth * 1));
                            vrt[2] = Avx.Add(vrt[2], Avx.LoadVector256(p + VectorWidth * 2));
                            vrt[3] = Avx.Add(vrt[3], Avx.LoadVector256(p + VectorWidth * 3));
                            vrt[4] = Avx.Add(vrt[4], Avx.LoadVector256(p + VectorWidth * 4));
                            vrt[5] = Avx.Add(vrt[5], Avx.LoadVector256(p + VectorWidth * 5));
                            vrt[6] = Avx.Add(vrt[6], Avx.LoadVector256(p + VectorWidth * 6));
                            vrt[7] = Avx.Add(vrt[7], Avx.LoadVector256(p + VectorWidth * 7));
                            vrt[8] = Avx.Add(vrt[8], Avx.LoadVector256(p + VectorWidth * 8));
                            vrt[9] = Avx.Add(vrt[9], Avx.LoadVector256(p + VectorWidth * 9));
                            vrt[10] = Avx.Add(vrt[10], Avx.LoadVector256(p + VectorWidth * 10));
                            vrt[11] = Avx.Add(vrt[11], Avx.LoadVector256(p + VectorWidth * 11));
                            vrt[12] = Avx.Add(vrt[12], Avx.LoadVector256(p + VectorWidth * 12));
                            vrt[13] = Avx.Add(vrt[13], Avx.LoadVector256(p + VectorWidth * 13));
                            vrt[14] = Avx.Add(vrt[14], Avx.LoadVector256(p + VectorWidth * 14));
                            vrt[15] = Avx.Add(vrt[15], Avx.LoadVector256(p + VectorWidth * 15));
                            p += nBlockWidth;
                        }
                        // Remainder processs.
                        for (i = 0; i < cntRem; ++i) {
                            rt += p[i];
                        }
                    }
                }
                // Reduce.
                for (i = 1; i < LoopUnrolling; ++i) {
                    vrt[0] = Avx.Add(vrt[0], vrt[i]); // vrt[0] += vrt[i]
                }
                for (i = 0; i < VectorWidth; ++i) {
                    rt += vrt[0].GetElement(i);
                }
                return rt;
            }
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// Sum - Vector AVX - Ptr - Loop unrolling *X.
        /// </summary>
        /// <param name="src">Soure array.</param>
        /// <param name="count">Soure array count.</param>
        /// <param name="count">Benchmark loops.</param>
        /// <param name="LoopUnrolling">The loopUnrolling.</param>
        /// <returns>Return the sum value.</returns>
        private static float SumVectorAvxPtrUX(float[] src, int count, int loops, int LoopUnrolling) {
#if Allow_Intrinsics && UNSAFE
            unsafe {
                float rt = 0; // Result.
                //const int LoopUnrolling = 16;
                if (LoopUnrolling <= 0) throw new ArgumentOutOfRangeException("LoopUnrolling", "Argument LoopUnrolling must >0 !");
                int VectorWidth = Vector256<float>.Count; // Block width.
                int nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
                int cntBlock = count / nBlockWidth; // Block count.
                int cntRem = count % nBlockWidth; // Remainder count.
                int i;
                //Vector256<float>[] vrt = new Vector256<float>[LoopUnrolling]; // Vector result.
                Vector256<float>* vrt = stackalloc Vector256<float>[LoopUnrolling]; ; // Vector result.
                for (i = 0; i < LoopUnrolling; ++i) {
                    vrt[i] = Vector256<float>.Zero;
                }
                float* p; // Pointer for src data.
                // Body.
                fixed (float* p0 = &src[0]) {
                    for (int j = 0; j < loops; ++j) {
                        p = p0;
                        // Vector processs.
                        for (i = 0; i < cntBlock; ++i) {
                            for(int k=0; k< LoopUnrolling; ++k) {
                                vrt[k] = Avx.Add(vrt[k], Avx.LoadVector256(p + VectorWidth * k)); // Add. vrt[k] += *(*__m256)(p+k);
                            }
                            p += nBlockWidth;
                        }
                        // Remainder processs.
                        for (i = 0; i < cntRem; ++i) {
                            rt += p[i];
                        }
                    }
                }
                // Reduce.
                for (i = 1; i < LoopUnrolling; ++i) {
                    vrt[0] = Avx.Add(vrt[0], vrt[i]); // vrt[0] += vrt[i]
                } // vrt = vrt + vrt1 + vrt2 + vrt3 + ... vrt15;
                for (i = 0; i < VectorWidth; ++i) {
                    rt += vrt[0].GetElement(i);
                }
                return rt;
            }
#else
            throw new NotSupportedException();
#endif
        }
    }
}
