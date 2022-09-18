# BenchmarkVector2
Using SIMD vector types to speed up floating-point array summation #2 (使用SIMD向量类型加速浮点数组求和运算 #2).

Article: https://www.cnblogs.com/zyl910/p/dotnet_simd_BenchmarkVector2.html

Project list: 
- BenchmarkVector: Shared Project.
- BenchmarkVectorCore20: .NETCoreApp 2.0 Console Application.
- BenchmarkVectorCore20UseLib: .NETCoreApp 2.0 Console Application use `BenchmarkVectorLib` Library(.NET Standard 2.0 Class Library).
- BenchmarkVectorCore30: .NETCoreApp 3.0 Console Application.
- BenchmarkVectorCpp: VC++ Console Application.
- BenchmarkVectorFw45: .NET Framework 4.5 Console Application.
- BenchmarkVectorFw46: .NET Framework 4.6.1 Console Application.
- BenchmarkVectorFw46UseLib: .NET Framework 4.6.1 Console Application use `BenchmarkVectorLib` Library(.NET Standard 2.0 Class Library).
- BenchmarkVectorLib: .NET Standard 2.0 Class Library.

Benchmark functions:
- SumBase: Sum - base.
- SumVector4: Sum - Vector4.
- SumVectorT: Sum - `Vector<T>`.
- SumVectorAvx: Sum - Vector AVX. Using Intrinsic to manipulate Vector256 type directly with the AVX instruction set
- SumVectorAvxSpan: Sum - Vector AVX - Span.
- SumVectorAvxPtr: Sum - Vector AVX - Ptr.

