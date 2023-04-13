using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class ByteArrayComparerBenchmarks
{
    private readonly byte[] array1;
    private readonly byte[] array2;

    public ByteArrayComparerBenchmarks()
    {
        array1 = Enumerable.Range(0, 1000000000).Select(i => (byte)i).ToArray();
        array2 = Enumerable.Range(0, 1000000000).Select(i => (byte)i).ToArray();
    }

    [Benchmark]
    public bool CompareWithLinq()
    {
        return array1.SequenceEqual(array2);
    }

    [Benchmark]
    public bool CompareWithUnsafeForLoop()
    {
        return AreByteArraysEqual(array1, array2);
    }
    
    private static unsafe bool AreByteArraysEqual(byte[] byteArray1, byte[] byteArray2)
    {
        if (byteArray1.Length != byteArray2.Length)
            return false;

        fixed (byte* p1 = byteArray1, p2 = byteArray2)
        {
            byte* x1 = p1, x2 = p2;
            int len = byteArray1.Length;

            while (len-- > 0)
            {
                if (*x1++ != *x2++)
                    return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool CompareWithForLoop()
    {
        if (array1.Length != array2.Length)
        {
            return false;
        }

        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] != array2[i])
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool CompareWithSimdThreadedLoop()
    {
        return AreByteArraysEqualSIMD(array1, array2);
    }
    
    public static unsafe bool AreByteArraysEqualSIMD(byte[] byteArray1, byte[] byteArray2)
    {
        if (byteArray1.Length != byteArray2.Length)
            return false;

        var length = byteArray1.Length;

        // Check if the arrays are small enough to compare serially
        if (length < Environment.ProcessorCount * 64)
        {
            return byteArray1.SequenceEqual(byteArray2);
        }

        // Compare chunks of the arrays in parallel using multiple threads and SIMD instructions
        var result = true;
        var chunkSize = Vector<byte>.Count * (length / (Environment.ProcessorCount * Vector<byte>.Count));
        Parallel.For(0, Environment.ProcessorCount, (j) =>
        {
            fixed (byte* p1 = byteArray1, p2 = byteArray2)
            {
                var start = j * chunkSize;
                var end = j == Environment.ProcessorCount - 1 ? length : (j + 1) * chunkSize;
                var i = start;
                var localResult = true;

                while (i < end - Vector<byte>.Count)
                {
                    byte* x1 = p1 + i, x2 = p2 + i;
                    var v1 = Unsafe.Read<Vector<byte>>(x1);
                    var v2 = Unsafe.Read<Vector<byte>>(x2);

                    if (Vector.EqualsAll(v1, v2) == false)
                    {
                        localResult = false;
                        break;
                    }

                    i += Vector<byte>.Count;
                }

                // Compare any remaining bytes
                while (i < end)
                {
                    byte* x1 = p1 + i, x2 = p2 + i;

                    if (*x1++ != *x2++)
                    {
                        localResult = false;
                        break;
                    }

                    i++;
                }

                if (localResult == false)
                {
                    result = false;
                }
            }
        });

        return result;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<ByteArrayComparerBenchmarks>();
    }
}