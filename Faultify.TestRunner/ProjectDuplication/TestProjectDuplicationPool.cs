using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Faultify.TestRunner.ProjectDuplication
{
    /// <summary>
    ///     A pool that grands access to a test project that can be used for mutation testing.
    /// </summary>
    public class TestProjectDuplicationPool
    {
        private static readonly object Lock = new object();
        private readonly AutoResetEvent _signalEvent = new AutoResetEvent(false);
        private readonly List<TestProjectDuplication> _testProjectDuplications;

        public TestProjectDuplicationPool(List<TestProjectDuplication> duplications)
        {
            _testProjectDuplications = duplications;

            foreach (var testProjectDuplication in _testProjectDuplications)
                testProjectDuplication.TestProjectFreed += OnTestProjectFreed;
        }

        /// <summary>
        ///     Takes and removes a test project from the pool
        /// </summary>
        /// <returns></returns>
        public TestProjectDuplication? TakeTestProject()
        {
            TestProjectDuplication? first = _testProjectDuplications.FirstOrDefault();
            if (first != null)
            {
                _testProjectDuplications.RemoveAt(0);
            }
            return first;
        }

        /// <summary>
        ///     Acquire a test project or wait until one is released.
        ///     This will hang until test projects are freed.
        /// </summary>
        /// <returns></returns>
        public TestProjectDuplication AcquireTestProject()
        {
            // Make sure only one thread can attempt to access a free project at a time.
            lock (Lock)
            {
                TestProjectDuplication? freeProject = GetFreeProject();

                if (freeProject != null) return freeProject;

                _signalEvent.WaitOne();

                freeProject = GetFreeProject();

                if (freeProject != null)
                {
                    return freeProject;
                }

                return AcquireTestProject();
            }
        }

        /// <summary>
        ///     Returns a free project or null if none exit.
        /// </summary>
        /// <returns></returns>
        public TestProjectDuplication? GetFreeProject()
        {
            TestProjectDuplication? project = _testProjectDuplications.First(x => !x.IsInUse);
            if (project != null)
            {
                project.IsInUse = true;
            }

            return project;
        }

        /// <summary>
        ///     Signal that a test test project is freed.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="_"></param>
        private void OnTestProjectFreed(object? e, TestProjectDuplication _)
        {
            _signalEvent.Set();
        }
    }
}
