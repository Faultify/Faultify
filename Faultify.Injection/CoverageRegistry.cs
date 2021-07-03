﻿using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using Faultify.TestRunner.Shared;
using NLog;

namespace Faultify.Injection
{
    /// <summary>
    ///     Registry that tracks which tests cover which entity handled.
    ///     This information is used by the test runner to know which mutations can be ran in parallel.
    /// </summary>
    public static class CoverageRegistry
    {
        private static readonly MutationCoverage MutationCoverage = new();
        private static string _currentTestCoverage = "NONE";
        private static readonly object RegisterMutex = new();
        private static MemoryMappedFile _mmf;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Is injected into <Module> by <see cref="TestCoverageInjector" /> and will be called on assembly load.
        /// </summary>
        public static void Initialize()
        {
            AppDomain.CurrentDomain.ProcessExit += OnCurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomain_ProcessExit;
            _mmf = MemoryMappedFile.OpenExisting("CoverageFile", MemoryMappedFileRights.ReadWrite);
        }

        private static void OnCurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                Utils.WriteMutationCoverageFile(MutationCoverage, _mmf);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Previously ignored Exception caught in CoverageRegistry: {0}");
                // ignored
            }
        }

        /// <summary>
        ///     Registers the given method entity handle as 'covered' by the last registered 'test'
        /// </summary>
        /// <param name="entityHandle"></param>
        public static void RegisterTargetCoverage(string assemblyName, int entityHandle)
        {
            lock (RegisterMutex)
            {
                try
                {
                    if (!MutationCoverage.Coverage.TryGetValue(_currentTestCoverage,
                        out List<RegisteredCoverage> targetHandles))
                    {
                        targetHandles = new List<RegisteredCoverage>();
                        MutationCoverage.Coverage[_currentTestCoverage] = targetHandles;
                    }

                    targetHandles.Add(new RegisteredCoverage(assemblyName, entityHandle));

                    Utils.WriteMutationCoverageFile(MutationCoverage, _mmf);
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex, "Previously ignored Exception-2 caught in CoverageRegistry: {0}");
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
