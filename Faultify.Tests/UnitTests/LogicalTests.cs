using System.IO;
using Faultify.Analyze.OpcodeAnalyzer;
using Faultify.Tests.UnitTests.Utils;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    internal class LogicalTests
    {
        private readonly string folder = Path.Combine("UnitTests", "TestSource", "LogicalTarget.cs");
        private readonly string nameSpace = "Faultify.Tests.UnitTests.TestSource.LogicalTarget";

        [Test]
        public void Logical_PreMutation_And()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.AndOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PostMutation_AndToOr()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            var mutatedBinary = DllTestHelper.MutateMethod<LogicalMutationAnalyzer>(binary, "AndOperator", OpCodes.Or);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.AndOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PostMutation_AndToXor01()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = false;

            // Act
            var mutatedBinary = DllTestHelper.MutateMethod<LogicalMutationAnalyzer>(binary, "AndOperator", OpCodes.Xor);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.AndOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PostMutation_AndToXor02()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            var mutatedBinary = DllTestHelper.MutateMethod<BitwiseMutationAnalyzer>(binary, "AndOperator", OpCodes.Xor);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.AndOperator(true, false);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PreMutation_Or()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.OrOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PostMutation_OrToAnd()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            var mutatedBinary = DllTestHelper.MutateMethod<LogicalMutationAnalyzer>(binary, "OrOperator", OpCodes.And);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.OrOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PostMutation_OrToXor()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = false;

            // Act
            var mutatedBinary = DllTestHelper.MutateMethod<LogicalMutationAnalyzer>(binary, "OrOperator", OpCodes.Xor);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.OrOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }
    }
}