# AreByteArraysEqual

// * Summary *

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1555/22H2/2022Update/SunValley2)
12th Gen Intel Core i7-12800H, 1 CPU, 20 logical and 14 physical cores
.NET SDK=6.0.408
  [Host]     : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.16 (6.0.1623.17311), X64 RyuJIT AVX2


|                      Method |      Mean |    Error |   StdDev |
|---------------------------- |----------:|---------:|---------:|
|             CompareWithLinq |  99.58 ms | 1.363 ms | 1.208 ms |
|    CompareWithUnsafeForLoop | 869.61 ms | 7.214 ms | 6.748 ms |
|          CompareWithForLoop | 865.98 ms | 3.968 ms | 3.518 ms |
| CompareWithSimdThreadedLoop |  32.60 ms | 0.285 ms | 0.266 ms |

// * Hints *
Outliers
  ByteArrayComparerBenchmarks.CompareWithLinq: Default    -> 1 outlier  was  removed (102.62 ms)
  ByteArrayComparerBenchmarks.CompareWithForLoop: Default -> 1 outlier  was  removed (882.89 ms)

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  1 ms   : 1 Millisecond (0.001 sec)

// ***** BenchmarkRunner: End *****
Run time: 00:01:36 (96.84 sec), executed benchmarks: 4

Global total time: 00:01:41 (101.29 sec), executed benchmarks: 4

