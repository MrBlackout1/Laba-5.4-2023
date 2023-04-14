using System;
using System.Threading;

class Program
{
    static int[,] matrix1;
    static int[,] matrix2;
    static int[,] resultMatrix;
    static Semaphore semaphore;
    static object lockObject = new object();
    static void Main(string[] args)
    {
        int size = 100;
        matrix1 = new int[size, size];
        matrix2 = new int[size, size];
        resultMatrix = new int[size, size];
        Random random = new Random();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix1[i, j] = random.Next(10);
                matrix2[i, j] = random.Next(10);
            }
        }
        int numThreads = 4;
        semaphore = new Semaphore(numThreads, numThreads);
        Thread[] threads = new Thread[numThreads];
        for (int i = 0; i < numThreads; i++)
        {
            threads[i] = new Thread(MultiplyMatrices);
            threads[i].Start(i);
        }
        for (int i = 0; i < numThreads; i++)
        {
            threads[i].Join();
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Console.Write(resultMatrix[i, j] + " ");
            }
            Console.WriteLine();
        }
    }
    static void MultiplyMatrices(object threadId)
    {
        int numThreads = 4;
        int size = 100;
        int startRow = ((int)threadId * size) / numThreads;
        int endRow = (((int)threadId + 1) * size) / numThreads - 1;

        for (int i = startRow; i <= endRow; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int sum = 0;
                for (int k = 0; k < size; k++)
                {
                    sum += matrix1[i, k] * matrix2[k, j];
                }
                semaphore.WaitOne();
                lock (lockObject)
                {
                    resultMatrix[i, j] = sum;
                }
                semaphore.Release();
            }
        }
    }
}