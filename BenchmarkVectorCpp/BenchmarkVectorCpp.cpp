// BenchmarkVectorCpp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <immintrin.h>
#include <malloc.h>
#include <stdio.h>
#include <time.h>

#ifndef EXCEPTION_EXECUTE_HANDLER 
#define EXCEPTION_EXECUTE_HANDLER (1)
#endif // !EXCEPTION_EXECUTE_HANDLER 

// Sum - base.
float SumBase(const float* src, int count, int loops) {
    float rt = 0;
    for (int j = 0; j < loops; ++j) {
        for (int i = 0; i < count; ++i) {
            rt += src[i];
        }
    }
    return rt;
}

// Sum - Vector AVX. Use _mm256_load_ps.
float SumVectorAvx(const float* src, int count, int loops) {
    float rt = 0;
    const int VectorWidth = sizeof(__m256) / sizeof(float);
    int vcount = count / VectorWidth;
    if (0 != count % VectorWidth) return rt;
    __m256 vrt = _mm256_setzero_ps();
    for (int j = 0; j < loops; ++j) {
        const float* p = src;
        for (int i = 0; i < vcount; ++i) {
            __m256 b = _mm256_load_ps(p);
            vrt = _mm256_add_ps(vrt, b);
            p += VectorWidth;
        }
    }
    // reduce.
    float dst[VectorWidth];
    _mm256_storeu_ps(dst, vrt);
    for (int i = 0; i < VectorWidth; ++i) {
        rt += dst[i];
    }
    return rt;
}

// Sum - Vector AVX use pointer.
float SumVectorAvxPtr(const float* src, int count, int loops) {
    float rt = 0;
    const int VectorWidth = sizeof(__m256) / sizeof(float);
    int vcount = count / VectorWidth;
    if (0 != count % VectorWidth) return rt;
    __m256 vrt = _mm256_setzero_ps();
    for (int j = 0; j < loops; ++j) {
        const __m256* p = (const __m256*)src;
        for (int i = 0; i < vcount; ++i) {
            vrt = _mm256_add_ps(vrt, *p);
            ++p;
        }
    }
    // reduce.
    float dst[VectorWidth];
    _mm256_storeu_ps(dst, vrt);
    for (int i = 0; i < VectorWidth; ++i) {
        rt += dst[i];
    }
    return rt;
}

// Do Benchmark.
void Benchmark() {
    const size_t alignment = 256 / 8; // sizeof(__m256) / sizeof(BYTE);
    // init.
    clock_t tickBegin, msUsed;
    double mFlops; // MFLOPS/s .
    double scale;
    float rt;
    const int count = 1024 * 4;
    const int loops = 1000 * 1000;
    //const int loops = 1;
    const double countMFlops = count * (double)loops / (1000.0 * 1000);
    float* src = (float*)_aligned_malloc(sizeof(float)*count, alignment); // new float[count];
    if (NULL == src) {
        printf("Memory alloc fail!");
        return;
    }
    for (int i = 0; i < count; ++i) {
        src[i] = (float)i;
    }
    printf("Benchmark: \tcount=%d, loops=%d, countMFlops=%f\n", count, loops, countMFlops);
    // SumBase.
    tickBegin = clock();
    rt = SumBase(src, count, loops);
    msUsed = clock() - tickBegin;
    mFlops = countMFlops * CLOCKS_PER_SEC / msUsed;
    printf("SumBase:\t%g\t# msUsed=%d, MFLOPS/s=%f\n", rt, (int)msUsed, mFlops);
    double mFlopsBase = mFlops;
    // SumVectorAvx.
    __try
    {
        tickBegin = clock();
        rt = SumVectorAvx(src, count, loops);
        msUsed = clock() - tickBegin;
        mFlops = countMFlops * CLOCKS_PER_SEC / msUsed;
        scale = mFlops / mFlopsBase;
        printf("SumVectorAvx:\t%g\t# msUsed=%d, MFLOPS/s=%f, scale=%f\n", rt, (int)msUsed, mFlops, scale);
        // SumVectorAvxPtr.
        tickBegin = clock();
        rt = SumVectorAvxPtr(src, count, loops);
        msUsed = clock() - tickBegin;
        mFlops = countMFlops * CLOCKS_PER_SEC / msUsed;
        scale = mFlops / mFlopsBase;
        printf("SumVectorAvxPtr:\t%g\t# msUsed=%d, MFLOPS/s=%f, scale=%f\n", rt, (int)msUsed, mFlops, scale);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        printf("Run SumVectorAvx fail!");
    }
    // done.
    _aligned_free(src);
}

int main() {
    printf("BenchmarkVectorCpp\n");
    printf("\n");
#ifdef _DEBUG
    printf("IsRelease:\tFalse\n");
#else
    printf("IsRelease:\tTrue\n");
#endif // _DEBUG
#ifdef _MSC_VER
    printf("_MSC_VER:\t%d\n", _MSC_VER);
#endif // _MSC_VER
#ifdef __AVX__
    printf("__AVX__:\t%d\n", __AVX__);
#endif // __AVX__
    printf("\n");
    // Benchmark.
    Benchmark();
}
