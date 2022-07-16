using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace VectorClassDemo {
    /// <summary>
    /// Demo for `Vector` static class.
    /// </summary>
    internal class VectorDemo {

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
            if (null == indent) indent = "";
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
            //tw.WriteLine(indent + string.Format("Vector<float>.Count:\t{0}\t# {1}bit", Vector<float>.Count, Vector<float>.Count * sizeof(float) * 8));
            //tw.WriteLine(indent + string.Format("Vector<double>.Count:\t{0}\t# {1}bit", Vector<double>.Count, Vector<double>.Count * sizeof(double) * 8));
            Assembly assembly;
            //assembly = typeof(Vector4).GetTypeInfo().Assembly;
            //tw.WriteLine(string.Format("Vector4.Assembly:\t{0}", assembly));
            //tw.WriteLine(string.Format("Vector4.Assembly.CodeBase:\t{0}", assembly.CodeBase));
            assembly = typeof(Vector<float>).GetTypeInfo().Assembly;
            tw.WriteLine(string.Format("Vector<T>.Assembly.CodeBase:\t{0}", assembly.CodeBase));
        }

        /// <summary>
        /// Run.
        /// </summary>
        /// <param name="tw">Output <see cref="TextWriter"/>.</param>
        /// <param name="indent">The indent.</param>
        public static void Run(TextWriter tw, string indent) {
            RunType(tw, indent, CreateVectorUseRotate(float.MinValue, float.PositiveInfinity, float.NaN, -1.2f, 0f, 1f, 2f, 4f), new Vector<float>(2.0f));
            RunType(tw, indent, CreateVectorUseRotate(double.MinValue, double.PositiveInfinity, -1.2, 0), new Vector<double>(2.0));
            RunType(tw, indent, CreateVectorUseRotate<sbyte>(sbyte.MinValue, sbyte.MaxValue, -1, 0, 1, 2, 3, 4), new Vector<sbyte>(2));
            RunType(tw, indent, CreateVectorUseRotate<short>(short.MinValue, short.MaxValue, -1, 0, 1, 2, 3, 4, 127, 128), new Vector<short>(2));
            RunType(tw, indent, CreateVectorUseRotate<int>(int.MinValue, int.MaxValue, -1, 0, 1, 2, 3, 32768), new Vector<int>(2));
            RunType(tw, indent, CreateVectorUseRotate<long>(long.MinValue, long.MaxValue, -1, 0, 1, 2, 3), new Vector<long>(2));
            RunType(tw, indent, CreateVectorUseRotate<byte>(byte.MinValue, byte.MaxValue, 0, 1, 2, 3, 4), new Vector<byte>(2));
            RunType(tw, indent, CreateVectorUseRotate<ushort>(ushort.MinValue, ushort.MaxValue, 0, 1, 2, 3, 4, 255, 256), new Vector<ushort>(2));
            RunType(tw, indent, CreateVectorUseRotate<uint>(uint.MinValue, uint.MaxValue, 0, 1, 2, 3, 65536), new Vector<uint>(2));
            RunType(tw, indent, CreateVectorUseRotate<ulong>(ulong.MinValue, ulong.MaxValue, 0, 1, 2, 3), new Vector<ulong>(2));
        }

        /// <summary>
        /// Create Vector&lt;T&gt; use rotate.
        /// </summary>
        /// <typeparam name="T">Vector type.</typeparam>
        /// <param name="list">Source value list.</param>
        /// <returns>Returns Vector&lt;T&gt;.</returns>
        static Vector<T> CreateVectorUseRotate<T>(params T[] list) where T : struct {
            if (null == list || list.Length <= 0) return Vector<T>.Zero;
            T[] arr = new T[Vector<T>.Count];
            int idx = 0;
            for(int i=0; i< arr.Length; ++i) {
                arr[i] = list[idx];
                ++idx;
                if (idx >= list.Length) idx = 0;
            }
            Vector <T> rt = new Vector<T>(arr);
            return rt;
        }

        /// <summary>
        /// Get hex string.
        /// </summary>
        /// <typeparam name="T">Vector value type.</typeparam>
        /// <param name="src">Source value.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="noFixEndian">No fix endian.</param>
        /// <returns>Returns hex string.</returns>
        private static string GetHex<T>(Vector<T> src, string separator, bool noFixEndian) where T : struct {
            Vector<byte> list = Vector.AsVectorByte(src);
            int unitCount = Vector<T>.Count;
            int unitSize = Vector<byte>.Count / unitCount;
            bool fixEndian = false;
            if (!noFixEndian && BitConverter.IsLittleEndian) fixEndian = true;
            StringBuilder sb = new StringBuilder();
            if (fixEndian) {
                // IsLittleEndian.
                for (int i=0; i < unitCount; ++i) {
                    if ((i > 0)) {
                        if (!string.IsNullOrEmpty(separator)) {
                            sb.Append(separator);
                        }
                    }
                    int idx = unitSize * (i+1) - 1;
                    for(int j = 0; j < unitSize; ++j) {
                        byte by = list[idx];
                        --idx;
                        sb.Append(by.ToString("X2"));
                    }
                }
            } else {
                for (int i = 0; i < Vector<byte>.Count; ++i) {
                    byte by = list[i];
                    if ((i > 0) && (0 == i % unitSize)) {
                        if (!string.IsNullOrEmpty(separator)) {
                            sb.Append(separator);
                        }
                    }
                    sb.Append(by.ToString("X2"));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// WriteLine wuth format.
        /// </summary>
        /// <typeparam name="T">Vector value type.</typeparam>
        /// <param name="tw">The TextWriter.</param>
        /// <param name="indent">The indent.</param>
        /// <param name="format">The format.</param>
        /// <param name="src">Source value</param>
        private static void WriteLineFormat<T>(TextWriter tw, string indent, string format, Vector<T> src) where T : struct {
            if (null == tw) return;
            string line = indent + string.Format(format, src);
            string hex = GetHex(src, " ", false);
            line += "\t# (" + hex +")";
            tw.WriteLine(line);
        }

        /// <summary>
        /// WriteLine wuth format.
        /// </summary>
        /// <param name="tw">The TextWriter.</param>
        /// <param name="indent">The indent.</param>
        /// <param name="format">The format.</param>
        /// <param name="src">Source value</param>
        private static void WriteLineFormat(TextWriter tw, string indent, string format, object src) {
            if (null == tw) return;
            tw.WriteLine(indent + string.Format(format, src));
        }

        /// <summary>
        /// Run type demo.
        /// </summary>
        /// <typeparam name="T">Vector type.</typeparam>
        /// <param name="tw">Output <see cref="TextWriter"/>.</param>
        /// <param name="indent">The indent.</param>
        /// <param name="srcT">Source temp value.</param>
        /// <param name="src2">Source 2.</param>
        static void RunType<T>(TextWriter tw, string indent, Vector<T> srcT, Vector<T> src2) where T : struct {
            Vector<T> src0 = Vector<T>.Zero;
            Vector<T> src1 = Vector<T>.One;
            Vector<T> srcAllOnes = ~Vector<T>.Zero;
            tw.WriteLine(indent + string.Format("-- {0}, Vector<{0}>.Count={1} --", typeof(T).Name, Vector<T>.Count));
            WriteLineFormat(tw, indent, "srcT:\t{0}", srcT);
            //WriteLineFormat(tw, indent, "src2:\t{0}", src2);
            WriteLineFormat(tw, indent, "srcAllOnes:\t{0}", srcAllOnes);

            // -- Methods --
            #region Methods
            //Abs<T>(Vector<T>) Returns a new vector whose elements are the absolute values of the given vector's elements.
            WriteLineFormat(tw, indent, "Abs(srcT):\t{0}", Vector.Abs(srcT));
            WriteLineFormat(tw, indent, "Abs(srcAllOnes):\t{0}", Vector.Abs(srcAllOnes));

            //Add<T>(Vector<T>, Vector<T>) Returns a new vector whose values are the sum of each pair of elements from two given vectors.
            WriteLineFormat(tw, indent, "Add(srcT, src1):\t{0}", Vector.Add(srcT, src1));
            WriteLineFormat(tw, indent, "Add(srcT, src2):\t{0}", Vector.Add(srcT, src2));

            //AndNot<T>(Vector<T>, Vector<T>) Returns a new vector by performing a bitwise And Not operation on each pair of corresponding elements in two vectors.
            WriteLineFormat(tw, indent, "AndNot(srcT, src1):\t{0}", Vector.AndNot(srcT, src1));
            WriteLineFormat(tw, indent, "AndNot(srcT, src2):\t{0}", Vector.AndNot(srcT, src2));

            //As<TFrom, TTo>(Vector<TFrom>)    Reinterprets aVector64<T> as a new Vector64<T>.
            //AsVectorByte<T>(Vector<T>) Reinterprets the bits of a specified vector into those of a vector of unsigned bytes.
            //AsVectorDouble<T>(Vector<T>)    Reinterprets the bits of a specified vector into those of a double-precision floating-point vector.
            //AsVectorInt16<T>(Vector<T>) Reinterprets the bits of a specified vector into those of a vector of 16-bit integers.
            //AsVectorInt32<T>(Vector<T>) Reinterprets the bits of a specified vector into those of a vector of integers.
            //AsVectorInt64<T>(Vector<T>) Reinterprets the bits of a specified vector into those of a vector of long integers.
            //AsVectorNInt<T>(Vector<T>)  Reinterprets the bits of a specified vector into those of a vector of native-sized integers.
            //AsVectorNUInt<T>(Vector<T>) Reinterprets the bits of a specified vector into those of a vector of native-sized, unsigned integers.
            //AsVectorSByte<T>(Vector<T>) Reinterprets the bits of a specified vector into those of a vector of signed bytes.
            //AsVectorSingle<T>(Vector<T>) Reinterprets the bits of a specified vector into those of a single-precision floating-point vector.
            //AsVectorUInt16<T>(Vector<T>)    Reinterprets the bits of a specified vector into those of a vector of unsigned 16-bit integers.
            //AsVectorUInt32<T>(Vector<T>)    Reinterprets the bits of a specified vector into those of a vector of unsigned integers.
            //AsVectorUInt64<T>(Vector<T>) Reinterprets the bits of a specified vector into those of a vector of unsigned long integers.
            // `As***` see below.

            //BitwiseAnd<T>(Vector<T>, Vector<T>) Returns a new vector by performing a bitwise Andoperation on each pair of elements in two vectors.
            WriteLineFormat(tw, indent, "BitwiseAnd(srcT, src1):\t{0}", Vector.BitwiseAnd(srcT, src1));
            WriteLineFormat(tw, indent, "BitwiseAnd(srcT, src2):\t{0}", Vector.BitwiseAnd(srcT, src2));
            //BitwiseOr<T>(Vector<T>, Vector<T>)  Returns a new vector by performing a bitwise Oroperation on each pair of elements in two vectors.
            WriteLineFormat(tw, indent, "BitwiseOr(srcT, src1):\t{0}", Vector.BitwiseOr(srcT, src1));
            WriteLineFormat(tw, indent, "BitwiseOr(srcT, src2):\t{0}", Vector.BitwiseOr(srcT, src2));

#if NET5_0_OR_GREATER
            //Ceiling(Vector<Double>) Returns a new vector whose elements are the smallest integral values that are greater than or equal to the given vector's elements.
            //Ceiling(Vector<Single>) Returns a new vector whose elements are the smallest integral values that are greater than or equal to the given vector's elements.
            if (typeof(T) == typeof(Double)) {
                WriteLineFormat(tw, indent, "Ceiling(srcT):\t{0}", Vector.Ceiling(Vector.AsVectorDouble(srcT)));
            } else if (typeof(T) == typeof(Single)) {
                WriteLineFormat(tw, indent, "Ceiling(srcT):\t{0}", Vector.Ceiling(Vector.AsVectorSingle(srcT)));
            }
#endif // NET5_0_OR_GREATER

            //ConditionalSelect(Vector<Int32>, Vector<Single>, Vector<Single>)    Creates a new single-precision vector with elements selected between two specified single-precision source vectors based on an integral mask vector.
            //ConditionalSelect(Vector<Int64>, Vector<Double>, Vector<Double>) Creates a new double-precision vector with elements selected between two specified double-precision source vectors based on an integral mask vector.
            //ConditionalSelect<T>(Vector<T>, Vector<T>, Vector<T>) Creates a new vector of a specified type with elements selected between two specified source vectors of the same type based on an integral mask vector.
            Vector<T> mask = Vector.GreaterThan(srcT, src0);
            WriteLineFormat(tw, indent, "# Max mask=GreaterThan(srcT, src0):\t{0}", mask);
            WriteLineFormat(tw, indent, "ConditionalSelect(mask, srcT, src0):\t{0}", Vector.ConditionalSelect(mask, srcT, src0));
            WriteLineFormat(tw, indent, "ConditionalSelect(srcT, src0, src1):\t{0}", Vector.ConditionalSelect(srcT, src0, src1));
            // ConditionalSelect = left&mask | right&~mask;
            //
            // Sample UInt32:
            //# srcT:   <0, 4294967295, 0, 1, 2, 3, 4, 0>       # (00000000 FFFFFFFF 00000000 00000001 00000002 00000003 00000004 00000000)
            //# ConditionalSelect(srcT, src0, src1):    <1, 0, 1, 0, 1, 0, 1, 1>        # (00000001 00000000 00000001 00000000 00000001 00000000 00000001 00000001)
            // Mean:
            //[0] = src0[0]&srcT[0] | src0[1]&~srcT[0] = 0&0 | 1&~0 = 0 | 1&0xFFFFFFFF = 1
            //[1] = src0[1]&srcT[1] | src0[1]&~srcT[1] = 0&4294967295 | 1&~4294967295 = 0 | 1&0 = 0
            //[2] = src0[2]&srcT[2] | src0[2]&~srcT[2] = 0&0 | 1&~0 = 0 | 1&0xFFFFFFFF = 1
            //[3] = src0[3]&srcT[3] | src0[3]&~srcT[3] = 0&1 | 1&~1 = 0 | 1&0xFFFFFFFE = 0
            //[4] = src0[4]&srcT[4] | src0[4]&~srcT[4] = 0&2 | 1&~2 = 0 | 1&0xFFFFFFFD = 1
            //[5] = src0[5]&srcT[5] | src0[5]&~srcT[5] = 0&3 | 1&~3 = 0 | 1&0xFFFFFFFC = 0
            //[6] = src0[6]&srcT[6] | src0[6]&~srcT[6] = 0&4 | 1&~4 = 0 | 1&0xFFFFFFFB = 1
            //[7] = src0[7]&srcT[7] | src0[7]&~srcT[7] = 0&0 | 1&~0 = 0 | 1 = 1

            //ConvertToDouble(Vector<Int64>) Converts a Vector<Int64>to aVector<Double>.
            //ConvertToDouble(Vector<UInt64>) Converts a Vector<UInt64> to aVector<Double>.
            //ConvertToInt32(Vector<Single>) Converts a Vector<Single> to aVector<Int32>.
            //ConvertToInt64(Vector<Double>) Converts a Vector<Double> to aVector<Int64>.
            //ConvertToSingle(Vector<Int32>) Converts a Vector<Int32> to aVector<Single>.
            //ConvertToSingle(Vector<UInt32>) Converts a Vector<UInt32> to aVector<Single>.
            //ConvertToUInt32(Vector<Single>) Converts a Vector<Single> to aVector<UInt32>.
            //ConvertToUInt64(Vector<Double>) Converts a Vector<Double> to aVector<UInt64>.
            // Infinity or NaN -> IntTypes.MinValue .
            if (typeof(T) == typeof(Double)) {
                WriteLineFormat(tw, indent, "ConvertToInt64(srcT):\t{0}", Vector.ConvertToInt64(Vector.AsVectorDouble(srcT)));
                WriteLineFormat(tw, indent, "ConvertToUInt64(srcT):\t{0}", Vector.ConvertToUInt64(Vector.AsVectorDouble(srcT)));
            } else if (typeof(T) == typeof(Single)) {
                WriteLineFormat(tw, indent, "ConvertToInt32(srcT):\t{0}", Vector.ConvertToInt32(Vector.AsVectorSingle(srcT)));
                WriteLineFormat(tw, indent, "ConvertToUInt32(srcT):\t{0}", Vector.ConvertToUInt32(Vector.AsVectorSingle(srcT)));
            } else if (typeof(T) == typeof(Int32)) {
                WriteLineFormat(tw, indent, "ConvertToSingle(srcT):\t{0}", Vector.ConvertToSingle(Vector.AsVectorInt32(srcT)));
            } else if (typeof(T) == typeof(UInt32)) {
                WriteLineFormat(tw, indent, "ConvertToSingle(srcT):\t{0}", Vector.ConvertToSingle(Vector.AsVectorUInt32(srcT)));
            } else if (typeof(T) == typeof(Int64)) {
                WriteLineFormat(tw, indent, "ConvertToDouble(srcT):\t{0}", Vector.ConvertToDouble(Vector.AsVectorInt64(srcT)));
            } else if (typeof(T) == typeof(UInt64)) {
                WriteLineFormat(tw, indent, "ConvertToDouble(srcT):\t{0}", Vector.ConvertToDouble(Vector.AsVectorUInt64(srcT)));
            }

            //Divide<T>(Vector<T>, Vector<T>) Returns a new vector whose values are the result of dividing the first vector's elements by the corresponding elements in the second vector.
            WriteLineFormat(tw, indent, "Divide(srcT, src2):\t{0}", Vector.Divide(srcT, src2));

            //Dot<T>(Vector<T>, Vector<T>) Returns the dot product of two vectors.
            WriteLineFormat(tw, indent, "Dot(srcT, src1):\t{0}", Vector.Dot(srcT, src1));
            WriteLineFormat(tw, indent, "Dot(srcT, src2):\t{0}", Vector.Dot(srcT, src2));
            WriteLineFormat(tw, indent, "Dot(src1, src2):\t{0}", Vector.Dot(src1, src2));

            //Equals(Vector<Double>, Vector<Double>)  Returns a new integral vector whose elements signal whether the elements in two specified double-precision vectors are equal.
            //Equals(Vector<Int32>, Vector<Int32>)    Returns a new integral vector whose elements signal whether the elements in two specified integral vectors are equal.
            //Equals(Vector<Int64>, Vector<Int64>)    Returns a new vector whose elements signal whether the elements in two specified long integer vectors are equal.
            //Equals(Vector<Single>, Vector<Single>) Returns a new integral vector whose elements signal whether the elements in two specified single-precision vectors are equal.
            //Equals<T>(Vector<T>, Vector<T>) Returns a new vector of a specified type whose elements signal whether the elements in two specified vectors of the same type are equal.
            WriteLineFormat(tw, indent, "Equals(srcT, src0):\t{0}", Vector.Equals(srcT, src0));
            WriteLineFormat(tw, indent, "Equals(srcT, src1):\t{0}", Vector.Equals(srcT, src1));

            //EqualsAll<T>(Vector<T>, Vector<T>) Returns a value that indicates whether each pair of elements in the given vectors is equal.
            WriteLineFormat(tw, indent, "EqualsAll(srcT, src0):\t{0}", Vector.EqualsAll(srcT, src0));
            //EqualsAny<T>(Vector<T>, Vector<T>) Returns a value that indicates whether any single pair of elements in the given vectors is equal.
            WriteLineFormat(tw, indent, "EqualsAny(srcT, src0):\t{0}", Vector.EqualsAny(srcT, src0));

#if NET5_0_OR_GREATER
            //Floor(Vector<Double>) Returns a new vector whose elements are the largest integral values that are less than or equal to the given vector's elements.
            //Floor(Vector<Single>)   Returns a new vector whose elements are the largest integral values that are less than or equal to the given vector's elements.
            if (typeof(T) == typeof(Double)) {
                WriteLineFormat(tw, indent, "Floor(srcT):\t{0}", Vector.Floor(Vector.AsVectorDouble(srcT)));
            } else if (typeof(T) == typeof(Single)) {
                WriteLineFormat(tw, indent, "Floor(srcT):\t{0}", Vector.Floor(Vector.AsVectorSingle(srcT)));
            }
#endif // NET5_0_OR_GREATER

            //GreaterThan(Vector<Double>, Vector<Double>) Returns a new integral vector whose elements signal whether the elements in one double-precision floating-point vector are greater than their corresponding elements in a second double-precision floating-point vector.
            //GreaterThan(Vector<Int32>, Vector<Int32>)   Returns a new integral vector whose elements signal whether the elements in one integral vector are greater than their corresponding elements in a second integral vector.
            //GreaterThan(Vector<Int64>, Vector<Int64>)   Returns a new long integer vector whose elements signal whether the elements in one long integer vector are greater than their corresponding elements in a second long integer vector.
            //GreaterThan(Vector<Single>, Vector<Single>) Returns a new integral vector whose elements signal whether the elements in one single-precision floating-point vector are greater than their corresponding elements in a second single-precision floating-point vector.
            //GreaterThan<T>(Vector<T>, Vector<T>)    Returns a new vector whose elements signal whether the elements in one vector of a specified type are greater than their corresponding elements in the second vector of the same time.
            WriteLineFormat(tw, indent, "GreaterThan(srcT, src0):\t{0}", Vector.GreaterThan(srcT, src0));
            WriteLineFormat(tw, indent, "GreaterThan(srcT, src1):\t{0}", Vector.GreaterThan(srcT, src1));

            //GreaterThanAll<T>(Vector<T>, Vector<T>) Returns a value that indicates whether all elements in the first vector are greater than the corresponding elements in the second vector.
            //GreaterThanAny<T>(Vector<T>, Vector<T>) Returns a value that indicates whether any element in the first vector is greater than the corresponding element in the second vector.

            //GreaterThanOrEqual(Vector<Double>, Vector<Double>) Returns a new integral vector whose elements signal whether the elements in one vector are greater than or equal to their corresponding elements in the second double-precision floating-point vector.
            //GreaterThanOrEqual(Vector<Int32>, Vector<Int32>)    Returns a new integral vector whose elements signal whether the elements in one integral vector are greater than or equal to their corresponding elements in the second integral vector.
            //GreaterThanOrEqual(Vector<Int64>, Vector<Int64>)    Returns a new long integer vector whose elements signal whether the elements in one long integer vector are greater than or equal to their corresponding elements in the second long integer vector.
            //GreaterThanOrEqual(Vector<Single>, Vector<Single>) Returns a new integral vector whose elements signal whether the elements in one vector are greater than or equal to their corresponding elements in the single-precision floating-point second vector.
            //GreaterThanOrEqual<T>(Vector<T>, Vector<T>) Returns a new vector whose elements signal whether the elements in one vector of a specified type are greater than or equal to their corresponding elements in the second vector of the same type.
            WriteLineFormat(tw, indent, "GreaterThanOrEqual(srcT, src0):\t{0}", Vector.GreaterThanOrEqual(srcT, src0));
            WriteLineFormat(tw, indent, "GreaterThanOrEqual(srcT, src1):\t{0}", Vector.GreaterThanOrEqual(srcT, src1));

            //GreaterThanOrEqualAll<T>(Vector<T>, Vector<T>) Returns a value that indicates whether all elements in the first vector are greater than or equal to all the corresponding elements in the second vector.
            //GreaterThanOrEqualAny<T>(Vector<T>, Vector<T>) Returns a value that indicates whether any element in the first vector is greater than or equal to the corresponding element in the second vector.

            //LessThan(Vector<Double>, Vector<Double>) Returns a new integral vector whose elements signal whether the elements in one double-precision floating-point vector are less than their corresponding elements in a second double-precision floating-point vector.
            //LessThan(Vector<Int32>, Vector<Int32>)  Returns a new integral vector whose elements signal whether the elements in one integral vector are less than their corresponding elements in a second integral vector.
            //LessThan(Vector<Int64>, Vector<Int64>)  Returns a new long integer vector whose elements signal whether the elements in one long integer vector are less than their corresponding elements in a second long integer vector.
            //LessThan(Vector<Single>, Vector<Single>) Returns a new integral vector whose elements signal whether the elements in one single-precision vector are less than their corresponding elements in a second single-precision vector.
            //LessThan<T>(Vector<T>, Vector<T>)   Returns a new vector of a specified type whose elements signal whether the elements in one vector are less than their corresponding elements in the second vector.
            WriteLineFormat(tw, indent, "LessThan(srcT, src0):\t{0}", Vector.LessThan(srcT, src0));
            WriteLineFormat(tw, indent, "LessThan(srcT, src1):\t{0}", Vector.LessThan(srcT, src1));

            //LessThanAll<T>(Vector<T>, Vector<T>) Returns a value that indicates whether all of the elements in the first vector are less than their corresponding elements in the second vector.
            //LessThanAny<T>(Vector<T>, Vector<T>) Returns a value that indicates whether any element in the first vector is less than the corresponding element in the second vector.

            //LessThanOrEqual(Vector<Double>, Vector<Double>) Returns a new integral vector whose elements signal whether the elements in one double-precision floating-point vector are less than or equal to their corresponding elements in a second double-precision floating-point vector.
            //LessThanOrEqual(Vector<Int32>, Vector<Int32>)   Returns a new integral vector whose elements signal whether the elements in one integral vector are less than or equal to their corresponding elements in a second integral vector.
            //LessThanOrEqual(Vector<Int64>, Vector<Int64>)   Returns a new long integer vector whose elements signal whether the elements in one long integer vector are less or equal to their corresponding elements in a second long integer vector.
            //LessThanOrEqual(Vector<Single>, Vector<Single>) Returns a new integral vector whose elements signal whether the elements in one single-precision floating-point vector are less than or equal to their corresponding elements in a second single-precision floating-point vector.
            //LessThanOrEqual<T>(Vector<T>, Vector<T>)    Returns a new vector whose elements signal whether the elements in one vector are less than or equal to their corresponding elements in the second vector.
            WriteLineFormat(tw, indent, "LessThanOrEqual(srcT, src0):\t{0}", Vector.LessThanOrEqual(srcT, src0));
            WriteLineFormat(tw, indent, "LessThanOrEqual(srcT, src1):\t{0}", Vector.LessThanOrEqual(srcT, src1));

            //LessThanOrEqualAll<T>(Vector<T>, Vector<T>) Returns a value that indicates whether all elements in the first vector are less than or equal to their corresponding elements in the second vector.
            //LessThanOrEqualAny<T>(Vector<T>, Vector<T>) Returns a value that indicates whether any element in the first vector is less than or equal to the corresponding element in the second vector.

            //Max<T>(Vector<T>, Vector<T>) Returns a new vector whose elements are the maximum of each pair of elements in the two given vectors.
            WriteLineFormat(tw, indent, "Max(srcT, src0):\t{0}", Vector.Max(srcT, src0));
            WriteLineFormat(tw, indent, "Max(srcT, src2):\t{0}", Vector.Max(srcT, src2));
            //Min<T>(Vector<T>, Vector<T>)    Returns a new vector whose elements are the minimum of each pair of elements in the two given vectors.
            WriteLineFormat(tw, indent, "Min(srcT, src0):\t{0}", Vector.Min(srcT, src0));
            WriteLineFormat(tw, indent, "Min(srcT, src2):\t{0}", Vector.Min(srcT, src2));
            WriteLineFormat(tw, indent, "Min(Max(srcT, src0), src2):\t{0}", Vector.Min(Vector.Max(srcT, src0), src2));

            //Multiply<T>(T, Vector<T>)   Returns a new vector whose values are a scalar value multiplied by each of the values of a specified vector.
            //Multiply<T>(Vector<T>, T) Returns a new vector whose values are the values of a specified vector each multiplied by a scalar value.
            //Multiply<T>(Vector<T>, Vector<T>)   Returns a new vector whose values are the product of each pair of elements in two specified vectors.
            WriteLineFormat(tw, indent, "Multiply(srcT, src2):\t{0}", Vector.Multiply(srcT, src2));

            //Narrow(Vector<Double>, Vector<Double>) Narrows two Vector<Double>instances into one Vector<Single>.
            //Narrow(Vector<Int16>, Vector<Int16>) Narrows two Vector<Int16> instances into one Vector<SByte>.
            //Narrow(Vector<Int32>, Vector<Int32>) Narrows two Vector<Int32> instances into one Vector<Int16>.
            //Narrow(Vector<Int64>, Vector<Int64>) Narrows two Vector<Int64> instances into one Vector<Int32>.
            //Narrow(Vector<UInt16>, Vector<UInt16>) Narrows two Vector<UInt16> instances into one Vector<Byte>.
            //Narrow(Vector<UInt32>, Vector<UInt32>) Narrows two Vector<UInt32> instances into one Vector<UInt16>.
            //Narrow(Vector<UInt64>, Vector<UInt64>) Narrows two Vector<UInt64> instances into one Vector<UInt32>.
            if (typeof(T) == typeof(Double)) {
                WriteLineFormat(tw, indent, "Narrow(srcT):\t{0}", Vector.Narrow(Vector.AsVectorDouble(srcT), Vector.AsVectorDouble(src1)));
            } else if (typeof(T) == typeof(Int16)) {
                WriteLineFormat(tw, indent, "Narrow(srcT):\t{0}", Vector.Narrow(Vector.AsVectorInt16(srcT), Vector.AsVectorInt16(src1)));
            } else if (typeof(T) == typeof(Int32)) {
                WriteLineFormat(tw, indent, "Narrow(srcT):\t{0}", Vector.Narrow(Vector.AsVectorInt32(srcT), Vector.AsVectorInt32(src1)));
            } else if (typeof(T) == typeof(Int64)) {
                WriteLineFormat(tw, indent, "Narrow(srcT):\t{0}", Vector.Narrow(Vector.AsVectorInt64(srcT), Vector.AsVectorInt64(src1)));
            } else if (typeof(T) == typeof(UInt16)) {
                WriteLineFormat(tw, indent, "Narrow(srcT):\t{0}", Vector.Narrow(Vector.AsVectorUInt16(srcT), Vector.AsVectorUInt16(src1)));
            } else if (typeof(T) == typeof(UInt32)) {
                WriteLineFormat(tw, indent, "Narrow(srcT):\t{0}", Vector.Narrow(Vector.AsVectorUInt32(srcT), Vector.AsVectorUInt32(src1)));
            } else if (typeof(T) == typeof(UInt64)) {
                WriteLineFormat(tw, indent, "Narrow(srcT):\t{0}", Vector.Narrow(Vector.AsVectorUInt64(srcT), Vector.AsVectorUInt64(src1)));
            }

            //Negate<T>(Vector<T>) Returns a new vector whose elements are the negation of the corresponding element in the specified vector.
            WriteLineFormat(tw, indent, "Negate(srcT):\t{0}", Vector.Negate(srcT));
            WriteLineFormat(tw, indent, "Negate(srcAllOnes):\t{0}", Vector.Negate(srcAllOnes));
            //OnesComplement<T>(Vector<T>) Returns a new vector whose elements are obtained by taking the one's complement of a specified vector's elements.
            WriteLineFormat(tw, indent, "OnesComplement(srcT):\t{0}", Vector.OnesComplement(srcT));
            WriteLineFormat(tw, indent, "OnesComplement(srcAllOnes):\t{0}", Vector.OnesComplement(srcAllOnes));

#if NET7_0_OR_GREATER
            //ShiftLeft(Vector<Byte>, Int32)  Shifts each element of a vector left by the specified amount.
            //ShiftLeft(Vector<Int16>, Int32) Shifts each element of a vector left by the specified amount.
            //ShiftLeft(Vector<Int32>, Int32) Shifts each element of a vector left by the specified amount.
            //ShiftLeft(Vector<Int64>, Int32) Shifts each element of a vector left by the specified amount.
            //ShiftLeft(Vector<IntPtr>, Int32)    Shifts each element of a vector left by the specified amount.
            //ShiftLeft(Vector<SByte>, Int32) Shifts each element of a vector left by the specified amount.
            //ShiftLeft(Vector<UInt16>, Int32)    Shifts each element of a vector left by the specified amount.
            //ShiftLeft(Vector<UInt32>, Int32) Shifts each element of a vector left by the specified amount.
            //ShiftLeft(Vector<UInt64>, Int32)    Shifts each element of a vector left by the specified amount.
            //ShiftLeft(Vector<UIntPtr>, Int32) Shifts each element of a vector left by the specified amount.
            //ShiftRightArithmetic(Vector<Int16>, Int32)  Shifts(signed) each element of a vector right by the specified amount.
            //ShiftRightArithmetic(Vector<Int32>, Int32)  Shifts(signed) each element of a vector right by the specified amount.
            //ShiftRightArithmetic(Vector<Int64>, Int32)  Shifts(signed) each element of a vector right by the specified amount.
            //ShiftRightArithmetic(Vector<IntPtr>, Int32) Shifts(signed) each element of a vector right by the specified amount.
            //ShiftRightArithmetic(Vector<SByte>, Int32)  Shifts(signed) each element of a vector right by the specified amount.
            //ShiftRightLogical(Vector<Byte>, Int32)  Shifts(unsigned) each element of a vector right by the specified amount.
            //ShiftRightLogical(Vector<Int16>, Int32) Shifts(unsigned) each element of a vector right by the specified amount.
            //ShiftRightLogical(Vector<Int32>, Int32) Shifts(unsigned) each element of a vector right by the specified amount.
            //ShiftRightLogical(Vector<Int64>, Int32) Shifts(unsigned) each element of a vector right by the specified amount.
            //ShiftRightLogical(Vector<IntPtr>, Int32)    Shifts(unsigned) each element of a vector right by the specified amount.
            //ShiftRightLogical(Vector<SByte>, Int32) Shifts(unsigned) each element of a vector right by the specified amount.
            //ShiftRightLogical(Vector<UInt16>, Int32)    Shifts(unsigned) each element of a vector right by the specified amount.
            //ShiftRightLogical(Vector<UInt32>, Int32)    Shifts(unsigned) each element of a vector right by the specified amount.
            //ShiftRightLogical(Vector<UInt64>, Int32)    Shifts(unsigned) each element of a vector right by the specified amount.
            //ShiftRightLogical(Vector<UIntPtr>, Int32)   Shifts(unsigned) each element of a vector right by the specified amount.
#endif // NET7_0_OR_GREATER

            //SquareRoot<T>(Vector<T>)    Returns a new vector whose elements are the square roots of a specified vector's elements.
            WriteLineFormat(tw, indent, "SquareRoot(srcT):\t{0}", Vector.SquareRoot(srcT));

            //Subtract<T>(Vector<T>, Vector<T>) Returns a new vector whose values are the difference between the elements in the second vector and their corresponding elements in the first vector.
            WriteLineFormat(tw, indent, "Subtract(srcT, src1):\t{0}", Vector.Subtract(srcT, src1));
            WriteLineFormat(tw, indent, "Subtract(srcT, src2):\t{0}", Vector.Subtract(srcT, src2));

#if NET6_0_OR_GREATER
            //Sum<T>(Vector<T>) Returns the sum of all the elements inside the specified vector.
            WriteLineFormat(tw, indent, "Sum(srcT):\t{0}", Vector.Sum(srcT));
            WriteLineFormat(tw, indent, "Sum(src1):\t{0}", Vector.Sum(src1));
#endif // NET6_0_OR_GREATER

            //Widen(Vector<Byte>, Vector<UInt16>, Vector<UInt16>) Widens aVector<Byte> into two Vector<UInt16>instances.
            //Widen(Vector<Int16>, Vector<Int32>, Vector<Int32>) Widens a Vector<Int16> into twoVector<Int32> instances.
            //Widen(Vector<Int32>, Vector<Int64>, Vector<Int64>) Widens a Vector<Int32> into twoVector<Int64> instances.
            //Widen(Vector<SByte>, Vector<Int16>, Vector<Int16>) Widens a Vector<SByte> into twoVector<Int16> instances.
            //Widen(Vector<Single>, Vector<Double>, Vector<Double>) Widens a Vector<Single> into twoVector<Double> instances.
            //Widen(Vector<UInt16>, Vector<UInt32>, Vector<UInt32>) Widens a Vector<UInt16> into twoVector<UInt32> instances.
            //Widen(Vector<UInt32>, Vector<UInt64>, Vector<UInt64>) Widens a Vector<UInt32> into twoVector<UInt64> instances.
            if (typeof(T) == typeof(Single)) {
                Vector<Double> low, high;
                Vector.Widen(Vector.AsVectorSingle(srcT), out low, out high);
                WriteLineFormat(tw, indent, "Widen(srcT).low:\t{0}", low);
                WriteLineFormat(tw, indent, "Widen(srcT).high:\t{0}", high);
            } else if (typeof(T) == typeof(SByte)) {
                Vector<Int16> low, high;
                Vector.Widen(Vector.AsVectorSByte(srcT), out low, out high);
                WriteLineFormat(tw, indent, "Widen(srcT).low:\t{0}", low);
                WriteLineFormat(tw, indent, "Widen(srcT).high:\t{0}", high);
            } else if (typeof(T) == typeof(Int16)) {
                Vector<Int32> low, high;
                Vector.Widen(Vector.AsVectorInt16(srcT), out low, out high);
                WriteLineFormat(tw, indent, "Widen(srcT).low:\t{0}", low);
                WriteLineFormat(tw, indent, "Widen(srcT).high:\t{0}", high);
            } else if (typeof(T) == typeof(Int32)) {
                Vector<Int64> low, high;
                Vector.Widen(Vector.AsVectorInt32(srcT), out low, out high);
                WriteLineFormat(tw, indent, "Widen(srcT).low:\t{0}", low);
                WriteLineFormat(tw, indent, "Widen(srcT).high:\t{0}", high);
            } else if (typeof(T) == typeof(Byte)) {
                Vector<UInt16> low, high;
                Vector.Widen(Vector.AsVectorByte(srcT), out low, out high);
                WriteLineFormat(tw, indent, "Widen(srcT).low:\t{0}", low);
                WriteLineFormat(tw, indent, "Widen(srcT).high:\t{0}", high);
            } else if (typeof(T) == typeof(UInt16)) {
                Vector<UInt32> low, high;
                Vector.Widen(Vector.AsVectorUInt16(srcT), out low, out high);
                WriteLineFormat(tw, indent, "Widen(srcT).low:\t{0}", low);
                WriteLineFormat(tw, indent, "Widen(srcT).high:\t{0}", high);
            } else if (typeof(T) == typeof(UInt32)) {
                Vector<UInt64> low, high;
                Vector.Widen(Vector.AsVectorUInt32(srcT), out low, out high);
                WriteLineFormat(tw, indent, "Widen(srcT).low:\t{0}", low);
                WriteLineFormat(tw, indent, "Widen(srcT).high:\t{0}", high);
            }

            //Xor<T>(Vector<T>, Vector<T>) Returns a new vector by performing a bitwise exclusive Or(XOr) operation on each pair of elements in two vectors.
            WriteLineFormat(tw, indent, "Xor(srcT, src1):\t{0}", Vector.Xor(srcT, src1));
            WriteLineFormat(tw, indent, "Xor(srcT, src2):\t{0}", Vector.Xor(srcT, src2));

            #endregion // Methods

            // done.
            tw.WriteLine();
        }
    }
}


