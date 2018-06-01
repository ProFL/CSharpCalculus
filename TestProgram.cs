// Written by Pedro F Linhares, 2018.
// Computer Engineering undergraduate at the University of Fortaleza.
// Contact: pedroflinhares@edu.unifor.br
//
// This code is released under the LGPLv3

using System;
using System.Diagnostics;

namespace Calculus
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            double result;

            Console.WriteLine("The following operation will be executed:");
            Console.WriteLine("\\int_{0}^{8}{(x^2 + 4*x + 4)dx");

            for (int i = 6; i <= 9; i++)
            {
                Integration.Step = Math.Pow(10, -i);
                sw.Start();
                result = Integration.ParallelIntegration((x) => x * x + 4 * x + x, 0, 8, 4);
                sw.Stop();

                Console.WriteLine($"Parallel integration = {result}, with precision 1e-{i} took ${sw.Elapsed} to finish.");

                sw.Reset();

                sw.Start();
                result = Integration.Integrate((x) => x * x + 4 * x + x, 0, 8);
                sw.Stop();

                Console.WriteLine($"Single-core integration = {result}, with precision 1e-{i} took ${sw.Elapsed} to finish.");

                sw.Reset();
            }

            Console.WriteLine("Benchmark finished.");
        }
    }
}
