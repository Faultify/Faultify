using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Faultify.Benchmark.Runner
{
    internal class Program
    {
        private static readonly double stryker_found_mutations = 124;
        private static readonly double faultify_found_mutations = 168;

        private static void Main(string[] args)
        {
            List<(int, long)> elapsedFaultify = BenchmarkFaultify();

            foreach ((int, long) elapse in elapsedFaultify)
            {
                double mps = faultify_found_mutations / TimeSpan.FromMilliseconds(elapse.Item2).Seconds;
                Console.WriteLine($"Runners: {elapse.Item1} Time: {elapse.Item2} Mps: {mps:0.00}");
            }

            List<(int, long)> elapsedStryker = BenchmarkStryker();

            foreach ((int, long) elapse in elapsedStryker)
            {
                double mps = stryker_found_mutations / TimeSpan.FromMilliseconds(elapse.Item2).Seconds;
                Console.WriteLine($"Runners: {elapse.Item1} Time: {elapse.Item2} Mps: {mps:0.00}");
            }
        }

        private static List<(int, long)> BenchmarkStryker()
        {
            List<(int, long)> elapsed = new List<(int, long)>();

            for (var i = 1; i < 7; i++)
            {
                var a = "\"['Faultify.Benchmark.NUnit\\\\Faultify.Benchmark.NUnit.csproj']\"";
                string strykerConfig =
                    $"stryker -tp {a} --project-file=Faultify.Benchmark\\Faultify.Benchmark.csproj -c " + i;

                Stopwatch st = Stopwatch.StartNew();

                Process process = new Process();
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

            for (var i = 1; i < 7; i++)
            {
                string faultifyConfig =
                    @" -t ..\..\..\..\Faultify.Benchmark.NUnit\Faultify.Benchmark.NUnit.csproj -f html -p " + i;

                Stopwatch st = Stopwatch.StartNew();

                Process process = Process.Start(@"..\..\..\..\..\Faultify.Cli\bin\Debug\netcoreapp3.1\Faultify.Cli.exe",
                    faultifyConfig);
                process.WaitForExit();

                st.Stop();
                elapsed.Add((i, st.ElapsedMilliseconds));
            }

            return elapsed;
        }
    }
}
