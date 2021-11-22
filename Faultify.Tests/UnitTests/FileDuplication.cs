using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faultify.TestRunner.ProjectDuplication;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    internal class FileDuplicationTests
    {
        [Test]
        public void TestPoolTake()
        {
            var pool = new TestProjectDuplicationPool(new List<TestProjectDuplication>
            {
                new TestProjectDuplication(null, null, 0)
            });

            Assert.AreEqual(pool.TakeTestProject().DuplicationNumber, 0);
            Assert.AreEqual(pool.TakeTestProject(), null);
        }

        [Test]
        public void AqcureTestProject()
        {
            var pool = new TestProjectDuplicationPool(new List<TestProjectDuplication>
            {
                new TestProjectDuplication(null, null, 0),
                new TestProjectDuplication(null, null, 1)
            });

            var project1 = pool.AcquireTestProject();
            var project2 = pool.AcquireTestProject();

            Assert.AreEqual(project1.DuplicationNumber, 0);
            Assert.AreEqual(project2.DuplicationNumber, 1);
        }

        [Test]
        public void AcquireTestProjectParallel()
        {
            var pool = new TestProjectDuplicationPool(new List<TestProjectDuplication>
            {
                new TestProjectDuplication(null, null, 0),
                new TestProjectDuplication(null, null, 1),
                new TestProjectDuplication(null, null, 2),
                new TestProjectDuplication(null, null, 3),
                new TestProjectDuplication(null, null, 4),
                new TestProjectDuplication(null, null, 5)
            });

            Parallel.ForEach(Enumerable.Range(0, 6), i =>
            {
                var project1 = pool.AcquireTestProject();
                Assert.NotNull(project1, null);
            });

            Assert.IsNull(pool.GetFreeProject());
        }

        [Test]
        public void AcquireTestProjectParallelAndFree()
        {
            var pool = new TestProjectDuplicationPool(new List<TestProjectDuplication>
            {
                new TestProjectDuplication(null, null, 0),
                new TestProjectDuplication(null, null, 1),
                new TestProjectDuplication(null, null, 2),
                new TestProjectDuplication(null, null, 3),
                new TestProjectDuplication(null, null, 4),
                new TestProjectDuplication(null, null, 5)
            });

            Parallel.ForEach(Enumerable.Range(0, 6), i =>
            {
                var project1 = pool.AcquireTestProject();
                Assert.NotNull(project1);
                project1.MarkAsFree();
            });

            Assert.IsNotNull(pool.GetFreeProject());
        }
    }
}