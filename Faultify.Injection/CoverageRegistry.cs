using System;
using System.Collections.Generic;
using System.IO;
using Faultify.TestRunner.Shared;

namespace Faultify.Injection
{
    /// <summary>
    ///     Registry that tracks which tests cover which entity handled.
    ///     This information is used by the test runner to know which mutations can be ran in parallel.
    /// </summary>
    public static class CoverageRegistry
    {
        private static readonly MutationCoverage MutationCoverage = new MutationCoverage();
        private static string _currentTestCoverage = "NONE";
        private static readonly object RegisterMutex = new object();

        /// <summary>
        ///     Is injected into <Module> by <see cref="TestCoverageInjector" /> and will be called on assembly load.
        /// </summary>
        public static void Initialize()
        {
            AppDomain.CurrentDomain.ProcessExit += OnCurrentDomain_ProcessExit;
        }

        private static void OnCurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                // Flush the coverage before process end.
                var serialized = MutationCoverage.Serialize();
                File.WriteAllBytes(TestRunnerConstants.CoverageFileName, serialized);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        ///     Registers the given method entity handle as 'covered' by the last registered 'test'
        /// </summary>
        /// <param name="entityHandle"></param>
        public static void RegisterTargetCoverage(int entityHandle)
        {
            lock (RegisterMutex)
            {
                try
                {
                    if (!MutationCoverage.Coverage.TryGetValue(_currentTestCoverage, out var targetHandles))
                    {
                        targetHandles = new List<int>();
                        MutationCoverage.Coverage[_currentTestCoverage] = targetHandles;
                    }

                    targetHandles.Add(entityHandle);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        ///     Registers the given test case as current method under test.
        /// </summary>
        /// <param name="testName"></param>
        public static void RegisterTestCoverage(string testName)
        {
            _currentTestCoverage = testName;
        }
    }
}