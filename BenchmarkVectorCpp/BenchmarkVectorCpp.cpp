// BenchmarkVectorCpp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <stdio.h>
#include <time.h>

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

// Do Benchmark.
void Benchmark() {
    // init.
    clock_t tickBegin, msUsed;
    double mFlops; // MFLOPS/s .
    double scale;
    float rt;
    const int count = 1024 * 4;
    const int loops = 1000 * 1000;
    //const int loops = 1;
    const double countMFlops = count * (double)loops / (1000.0 * 1000);
    float* const src = new float[count];
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
    //double mFlopsBase = mFlops;
    // done.
    delete[] src;
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
