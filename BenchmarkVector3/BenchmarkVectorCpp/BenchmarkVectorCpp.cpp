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
float SumBase(const float* src, size_t count, int loops) {
    float rt = 0; // Result.
    size_t i;
    for (int j = 0; j < loops; ++j) {
        for (i = 0; i < count; ++i) {
            rt += src[i];
        }
    }
    return rt;
}

// Sum - base - Loop unrolling *4.
float SumBaseU4(const float* src, size_t count, int loops) {
    float rt = 0; // Result.
    float rt1=0;
    float rt2 = 0;
    float rt3 = 0;
    size_t nBlockWidth = 4; // Block width.
    size_t cntBlock = count / nBlockWidth; // Block count.
    size_t cntRem = count % nBlockWidth; // Remainder count.
    size_t p; // Index for src data.
    size_t i;
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
        for (i = 0; i < cntRem; ++i) {
            rt += src[p + i];
        }
    }
    // Reduce.
    rt = rt + rt1 + rt2 + rt3;
    return rt;
}

// Sum - Vector AVX.
float SumVectorAvx(const float* src, size_t count, int loops) {
    float rt = 0; // Result.
    size_t VectorWidth = sizeof(__m256) / sizeof(float); // Block width.
    size_t nBlockWidth = VectorWidth; // Block width.
    size_t cntBlock = count / nBlockWidth; // Block count.
    size_t cntRem = count % nBlockWidth; // Remainder count.
    __m256 vrt = _mm256_setzero_ps(); // Vector result. [AVX] Set zero.
    __m256 vload; // Vector load.
    const float* p; // Pointer for src data.
    size_t i;
    // Body.
    for (int j = 0; j < loops; ++j) {
        p = src;
        // Vector processs.
        for (i = 0; i < cntBlock; ++i) {
            vload = _mm256_load_ps(p);    // Load. vload = *(*__m256)p;
            vrt = _mm256_add_ps(vrt, vload);    // Add. vrt += vload;
            p += nBlockWidth;
        }
        // Remainder processs.
        for (i = 0; i < cntRem; ++i) {
            rt += p[i];
        }
    }
    // Reduce.
    p = (const float*)&vrt;
    for (i = 0; i < VectorWidth; ++i) {
        rt += p[i];
    }
    return rt;
}

// Sum - Vector AVX - Loop unrolling *4.
float SumVectorAvxU4(const float* src, size_t count, int loops) {
    float rt = 0;    // Result.
    const int LoopUnrolling = 4;
    size_t VectorWidth = sizeof(__m256) / sizeof(float); // Block width.
    size_t nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
    size_t cntBlock = count / nBlockWidth; // Block count.
    size_t cntRem = count % nBlockWidth; // Remainder count.
    __m256 vrt = _mm256_setzero_ps(); // Vector result. [AVX] Set zero.
    __m256 vrt1 = _mm256_setzero_ps();
    __m256 vrt2 = _mm256_setzero_ps();
    __m256 vrt3 = _mm256_setzero_ps();
    __m256 vload; // Vector load.
    __m256 vload1, vload2, vload3;
    const float* p; // Pointer for src data.
    size_t i;
    // Body.
    for (int j = 0; j < loops; ++j) {
        p = src;
        // Block processs.
        for (i = 0; i < cntBlock; ++i) {
            vload = _mm256_load_ps(p);    // Load. vload = *(*__m256)p;
            vload1 = _mm256_load_ps(p + VectorWidth * 1);
            vload2 = _mm256_load_ps(p + VectorWidth * 2);
            vload3 = _mm256_load_ps(p + VectorWidth * 3);
            vrt = _mm256_add_ps(vrt, vload);    // Add. vrt += vload;
            vrt1 = _mm256_add_ps(vrt1, vload1);
            vrt2 = _mm256_add_ps(vrt2, vload2);
            vrt3 = _mm256_add_ps(vrt3, vload3);
            p += nBlockWidth;
        }
        // Remainder processs.
        for (i = 0; i < cntRem; ++i) {
            rt += p[i];
        }
    }
    // Reduce.
    vrt = _mm256_add_ps(_mm256_add_ps(vrt, vrt1), _mm256_add_ps(vrt2, vrt3)); // vrt = vrt + vrt1 + vrt2 + vrt3;
    p = (const float*)&vrt;
    for (i = 0; i < VectorWidth; ++i) {
        rt += p[i];
    }
    return rt;
}

// Sum - Vector AVX - Loop unrolling *16.
float SumVectorAvxU16(const float* src, size_t count, int loops) {
    float rt = 0;    // Result.
    const int LoopUnrolling = 16;
    size_t VectorWidth = sizeof(__m256) / sizeof(float); // Block width.
    size_t nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
    size_t cntBlock = count / nBlockWidth; // Block count.
    size_t cntRem = count % nBlockWidth; // Remainder count.
    __m256 vrt = _mm256_setzero_ps(); // Vector result. [AVX] Set zero.
    __m256 vrt1 = _mm256_setzero_ps();
    __m256 vrt2 = _mm256_setzero_ps();
    __m256 vrt3 = _mm256_setzero_ps();
    __m256 vrt4 = _mm256_setzero_ps();
    __m256 vrt5 = _mm256_setzero_ps();
    __m256 vrt6 = _mm256_setzero_ps();
    __m256 vrt7 = _mm256_setzero_ps();
    __m256 vrt8 = _mm256_setzero_ps();
    __m256 vrt9 = _mm256_setzero_ps();
    __m256 vrt10 = _mm256_setzero_ps();
    __m256 vrt11 = _mm256_setzero_ps();
    __m256 vrt12 = _mm256_setzero_ps();
    __m256 vrt13 = _mm256_setzero_ps();
    __m256 vrt14 = _mm256_setzero_ps();
    __m256 vrt15 = _mm256_setzero_ps();
    const float* p; // Pointer for src data.
    size_t i;
    // Body.
    for (int j = 0; j < loops; ++j) {
        p = src;
        // Block processs.
        for (i = 0; i < cntBlock; ++i) {
            vrt = _mm256_add_ps(vrt, _mm256_load_ps(p));    // Add. vrt += *((*__m256)(p)+k);
            vrt1 = _mm256_add_ps(vrt1, _mm256_load_ps(p + VectorWidth * 1));
            vrt2 = _mm256_add_ps(vrt2, _mm256_load_ps(p + VectorWidth * 2));
            vrt3 = _mm256_add_ps(vrt3, _mm256_load_ps(p + VectorWidth * 3));
            vrt4 = _mm256_add_ps(vrt4, _mm256_load_ps(p + VectorWidth * 4));
            vrt5 = _mm256_add_ps(vrt5, _mm256_load_ps(p + VectorWidth * 5));
            vrt6 = _mm256_add_ps(vrt6, _mm256_load_ps(p + VectorWidth * 6));
            vrt7 = _mm256_add_ps(vrt7, _mm256_load_ps(p + VectorWidth * 7));
            vrt8 = _mm256_add_ps(vrt8, _mm256_load_ps(p + VectorWidth * 8));
            vrt9 = _mm256_add_ps(vrt9, _mm256_load_ps(p + VectorWidth * 9));
            vrt10 = _mm256_add_ps(vrt10, _mm256_load_ps(p + VectorWidth * 10));
            vrt11 = _mm256_add_ps(vrt11, _mm256_load_ps(p + VectorWidth * 11));
            vrt12 = _mm256_add_ps(vrt12, _mm256_load_ps(p + VectorWidth * 12));
            vrt13 = _mm256_add_ps(vrt13, _mm256_load_ps(p + VectorWidth * 13));
            vrt14 = _mm256_add_ps(vrt14, _mm256_load_ps(p + VectorWidth * 14));
            vrt15 = _mm256_add_ps(vrt15, _mm256_load_ps(p + VectorWidth * 15));
            p += nBlockWidth;
        }
        // Remainder processs.
        for (i = 0; i < cntRem; ++i) {
            rt += p[i];
        }
    }
    // Reduce.
    vrt = _mm256_add_ps(_mm256_add_ps(_mm256_add_ps(_mm256_add_ps(vrt, vrt1), _mm256_add_ps(vrt2, vrt3))
        , _mm256_add_ps(_mm256_add_ps(vrt4, vrt5), _mm256_add_ps(vrt6, vrt7)))
        , _mm256_add_ps(_mm256_add_ps(_mm256_add_ps(vrt8, vrt9), _mm256_add_ps(vrt10, vrt11))
        , _mm256_add_ps(_mm256_add_ps(vrt12, vrt13), _mm256_add_ps(vrt14, vrt15))))
    ; // vrt = vrt + vrt1 + vrt2 + vrt3 + ... vrt15;
    p = (const float*)&vrt;
    for (i = 0; i < VectorWidth; ++i) {
        rt += p[i];
    }
    return rt;
}

// Sum - Vector AVX - Loop unrolling *16 - Array.
float SumVectorAvxU16A(const float* src, size_t count, int loops) {
    float rt = 0;    // Result.
    const int LoopUnrolling = 16;
    size_t VectorWidth = sizeof(__m256) / sizeof(float); // Block width.
    size_t nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
    size_t cntBlock = count / nBlockWidth; // Block count.
    size_t cntRem = count % nBlockWidth; // Remainder count.
    size_t i;
    __m256 vrt[LoopUnrolling]; // Vector result.
    for (i = 0; i < LoopUnrolling; ++i) {
        vrt[i] = _mm256_setzero_ps(); // [AVX] Set zero.
    }
    const float* p; // Pointer for src data.
    // Body.
    for (int j = 0; j < loops; ++j) {
        p = src;
        // Block processs.
        for (i = 0; i < cntBlock; ++i) {
            vrt[0] = _mm256_add_ps(vrt[0], _mm256_load_ps(p));    // Add. vrt += *((*__m256)(p)+k);
            vrt[1] = _mm256_add_ps(vrt[1], _mm256_load_ps(p + VectorWidth * 1));
            vrt[2] = _mm256_add_ps(vrt[2], _mm256_load_ps(p + VectorWidth * 2));
            vrt[3] = _mm256_add_ps(vrt[3], _mm256_load_ps(p + VectorWidth * 3));
            vrt[4] = _mm256_add_ps(vrt[4], _mm256_load_ps(p + VectorWidth * 4));
            vrt[5] = _mm256_add_ps(vrt[5], _mm256_load_ps(p + VectorWidth * 5));
            vrt[6] = _mm256_add_ps(vrt[6], _mm256_load_ps(p + VectorWidth * 6));
            vrt[7] = _mm256_add_ps(vrt[7], _mm256_load_ps(p + VectorWidth * 7));
            vrt[8] = _mm256_add_ps(vrt[8], _mm256_load_ps(p + VectorWidth * 8));
            vrt[9] = _mm256_add_ps(vrt[9], _mm256_load_ps(p + VectorWidth * 9));
            vrt[10] = _mm256_add_ps(vrt[10], _mm256_load_ps(p + VectorWidth * 10));
            vrt[11] = _mm256_add_ps(vrt[11], _mm256_load_ps(p + VectorWidth * 11));
            vrt[12] = _mm256_add_ps(vrt[12], _mm256_load_ps(p + VectorWidth * 12));
            vrt[13] = _mm256_add_ps(vrt[13], _mm256_load_ps(p + VectorWidth * 13));
            vrt[14] = _mm256_add_ps(vrt[14], _mm256_load_ps(p + VectorWidth * 14));
            vrt[15] = _mm256_add_ps(vrt[15], _mm256_load_ps(p + VectorWidth * 15));
            p += nBlockWidth;
        }
        // Remainder processs.
        for (i = 0; i < cntRem; ++i) {
            rt += p[i];
        }
    }
    // Reduce.
    vrt[0] = _mm256_add_ps(_mm256_add_ps(_mm256_add_ps(_mm256_add_ps(vrt[0], vrt[1]), _mm256_add_ps(vrt[2], vrt[3]))
        , _mm256_add_ps(_mm256_add_ps(vrt[4], vrt[5]), _mm256_add_ps(vrt[6], vrt[7])))
        , _mm256_add_ps(_mm256_add_ps(_mm256_add_ps(vrt[8], vrt[9]), _mm256_add_ps(vrt[10], vrt[11]))
        , _mm256_add_ps(_mm256_add_ps(vrt[12], vrt[13]), _mm256_add_ps(vrt[14], vrt[15]))))
    ; // vrt = vrt + vrt1 + vrt2 + vrt3 + ... vrt15;
    p = (const float*)&vrt;
    for (i = 0; i < VectorWidth; ++i) {
        rt += p[i];
    }
    return rt;
}

// Sum - Vector AVX - Loop unrolling *X - Array.
float SumVectorAvxUX(const float* src, size_t count, int loops, const int LoopUnrolling) {
    float rt = 0;    // Result.
    size_t VectorWidth = sizeof(__m256) / sizeof(float); // Block width.
    size_t nBlockWidth = VectorWidth * LoopUnrolling; // Block width.
    size_t cntBlock = count / nBlockWidth; // Block count.
    size_t cntRem = count % nBlockWidth; // Remainder count.
    size_t i;
    __m256* vrt = new __m256[LoopUnrolling]; // Vector result.
    for (i = 0; i < LoopUnrolling; ++i) {
        vrt[i] = _mm256_setzero_ps(); // [AVX] Set zero.
    }
    const float* p; // Pointer for src data.
    // Body.
    for (int j = 0; j < loops; ++j) {
        p = src;
        // Block processs.
        for (i = 0; i < cntBlock; ++i) {
            for (int k = 0; k < LoopUnrolling; ++k) {
                vrt[k] = _mm256_add_ps(vrt[k], _mm256_load_ps(p + VectorWidth * k));    // Add. vrt += *((*__m256)(p)+k);
            }
            p += nBlockWidth;
        }
        // Remainder processs.
        for (i = 0; i < cntRem; ++i) {
            rt += p[i];
        }
    }
    // Reduce.
    for (i = 1; i < LoopUnrolling; ++i) {
        vrt[0] = _mm256_add_ps(vrt[0], vrt[i]); // vrt[0] += vrt[i]
    } // vrt = vrt + vrt1 + vrt2 + vrt3 + ... vrt15;
    p = (const float*)&vrt[0];
    for (i = 0; i < VectorWidth; ++i) {
        rt += p[i];
    }
    delete[] vrt;
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
    // SumBaseU4.
    tickBegin = clock();
    rt = SumBaseU4(src, count, loops);
    msUsed = clock() - tickBegin;
    mFlops = countMFlops * CLOCKS_PER_SEC / msUsed;
    scale = mFlops / mFlopsBase;
    printf("SumBaseU4:\t%g\t# msUsed=%d, MFLOPS/s=%f, scale=%f\n", rt, (int)msUsed, mFlops, scale);
    // SumVectorAvx.
    __try {
        tickBegin = clock();
        rt = SumVectorAvx(src, count, loops);
        msUsed = clock() - tickBegin;
        mFlops = countMFlops * CLOCKS_PER_SEC / msUsed;
        scale = mFlops / mFlopsBase;
        printf("SumVectorAvx:\t%g\t# msUsed=%d, MFLOPS/s=%f, scale=%f\n", rt, (int)msUsed, mFlops, scale);
        // SumVectorAvxU4.
        tickBegin = clock();
        rt = SumVectorAvxU4(src, count, loops);
        msUsed = clock() - tickBegin;
        mFlops = countMFlops * CLOCKS_PER_SEC / msUsed;
        scale = mFlops / mFlopsBase;
        printf("SumVectorAvxU4:\t%g\t# msUsed=%d, MFLOPS/s=%f, scale=%f\n", rt, (int)msUsed, mFlops, scale);
        // SumVectorAvxU16.
        tickBegin = clock();
        rt = SumVectorAvxU16(src, count, loops);
        msUsed = clock() - tickBegin;
        mFlops = countMFlops * CLOCKS_PER_SEC / msUsed;
        scale = mFlops / mFlopsBase;
        printf("SumVectorAvxU16:\t%g\t# msUsed=%d, MFLOPS/s=%f, scale=%f\n", rt, (int)msUsed, mFlops, scale);
        // SumVectorAvxU16A.
        tickBegin = clock();
        rt = SumVectorAvxU16A(src, count, loops);
        msUsed = clock() - tickBegin;
        mFlops = countMFlops * CLOCKS_PER_SEC / msUsed;
        scale = mFlops / mFlopsBase;
        printf("SumVectorAvxU16A:\t%g\t# msUsed=%d, MFLOPS/s=%f, scale=%f\n", rt, (int)msUsed, mFlops, scale);
        // SumVectorAvxUX.
        int LoopUnrollingArray[] = { 4, 8, 16 };
        for (int i = 0; i < sizeof(LoopUnrollingArray) / sizeof(LoopUnrollingArray[0]); ++i) {
            int loopUnrolling = LoopUnrollingArray[i];
            tickBegin = clock();
            rt = SumVectorAvxUX(src, count, loops, loopUnrolling);
            msUsed = clock() - tickBegin;
            mFlops = countMFlops * CLOCKS_PER_SEC / msUsed;
            scale = mFlops / mFlopsBase;
            printf("SumVectorAvxUX[%d]:\t%g\t# msUsed=%d, MFLOPS/s=%f, scale=%f\n", loopUnrolling, rt, (int)msUsed, mFlops, scale);
        }
    }
    __except (EXCEPTION_EXECUTE_HANDLER) {
        printf("Run SumVectorAvx fail!");
    }
    // done.
    _aligned_free(src);
}

int main() {
    printf("BenchmarkVectorCpp\n");
    printf("\n");
    printf("Pointer size:\t%d\n", (int)(sizeof(void*)));
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
