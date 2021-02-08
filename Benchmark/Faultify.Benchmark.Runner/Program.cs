﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Faultify.Benchmark.Runner
{
    class Program
    {
        private static double stryker_found_mutations = 31.0;
        private static double faultify_found_mutations = 29;

        static void Main(string[] args)
        {
            var elapsedFaultify = BenchmarkFaultify();
            
            foreach (var elapse in elapsedFaultify)
            {
                double mps = (faultify_found_mutations / TimeSpan.FromMilliseconds(elapse.Item2).Seconds);
                Console.WriteLine($"Threads: {elapse.Item1} Time: {elapse.Item2} Mps: {mps:0.00}");
            }

            var elapsedStryker = BenchmarkStryker();

            foreach (var elapse in elapsedStryker)
            {
                double mps = (stryker_found_mutations / TimeSpan.FromMilliseconds(elapse.Item2).Seconds);
                Console.WriteLine($"Threads: {elapse.Item1} Time: {elapse.Item2} Mps: {mps:0.00}");
            }


            Console.WriteLine("Hello World!");
        }

        private static List<(int, long)> BenchmarkStryker()
        {
            List<(int, long)> elapsed = new List<(int, long)>();

            for (int i = 1; i < 7; i++)
            {
                string a = "\"['Faultify.Benchmark.NUnit\\\\Faultify.Benchmark.NUnit.csproj']\"";
                string strykerConfig = $"stryker -tp {a} --project-file=Faultify.Benchmark\\Faultify.Benchmark.csproj -c " + i;

                var st = Stopwatch.StartNew();
                Console.WriteLine(strykerConfig);
                var process = new Process();
                process.StartInfo = new ProcessStartInfo("dotnet", strykerConfig);
                process.StartInfo.WorkingDirectory = @"..\..\..\..\";

                process.Start();

                process.WaitForExit();

                st.Stop();
                elapsed.Add((i, st.ElapsedMilliseconds));
            }

            return elapsed;
        }

        private static List<(int, long)> BenchmarkFaultify()
        {
            List<(int, long)> elapsed = new List<(int, long)>();

            for (int i = 1; i < 7; i++)
            {
                string faultifyConfig = @" -t ..\..\..\..\Faultify.Benchmark.NUnit\Faultify.Benchmark.NUnit.csproj -f html -p " + i;

                var st = Stopwatch.StartNew();

                var process = Process.Start(@"..\..\..\..\..\Faultify.Cli\bin\Debug\netcoreapp3.1\Faultify.Cli.exe", faultifyConfig);
                process.WaitForExit();

                st.Stop();
                elapsed.Add((i, st.ElapsedMilliseconds));
            }

            return elapsed;
        }
    }
}
