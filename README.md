# BenchmarkVector
Benchmark of .NET SIMD Vector types(Vector, Vector&lt;T>) (.NET SIMD 向量类型的基准测试)

Project list: 
- BenchmarkVector: Shared Project.
- BenchmarkVectorCore20: .NETCoreApp 2.0 Console Application.
- BenchmarkVectorCore20UseLib: .NETCoreApp 2.0 Console Application use `BenchmarkVectorLib` Library(.NET Standard 2.0 Class Library).
- BenchmarkVectorFw45: .NET Framework 4.5 Console Application.
- BenchmarkVectorFw46: .NET Framework 4.6.1 Console Application.
- BenchmarkVectorFw46UseLib: .NET Framework 4.6.1 Console Application use `BenchmarkVectorLib` Library(.NET Standard 2.0 Class Library).
- BenchmarkVectorLib: .NET Standard 2.0 Class Library.

Benchmark functions:
- SumBase: Sum - base.
- SumVector4: Sum - Vector4.
- SumVectorT: Sum - Vector<T>.

