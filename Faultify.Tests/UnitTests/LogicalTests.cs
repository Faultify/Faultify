extern alias MC;
using System.IO;
using Faultify.Analyze.Analyzers;
using Faultify.Tests.UnitTests.Utils;
using MC::Mono.Cecil.Cil;
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
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                dynamic instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.AndOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PostMutation_AndToOr()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateMethod<BitwiseAnalyzer>(binary, "AndOperator", OpCodes.Or);
            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                dynamic instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.AndOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PostMutation_AndToXor01()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = false;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateMethod<BitwiseAnalyzer>(binary, "AndOperator", OpCodes.Xor);
            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                dynamic instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.AndOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PostMutation_AndToXor02()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateMethod<BitwiseAnalyzer>(binary, "AndOperator", OpCodes.Xor);
            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                dynamic instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.AndOperator(true, false);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PreMutation_Or()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                dynamic instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.OrOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PostMutation_OrToAnd()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateMethod<BitwiseAnalyzer>(binary, "OrOperator", OpCodes.And);
            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                dynamic instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.OrOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Logical_PostMutation_OrToXor()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = false;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateMethod<BitwiseAnalyzer>(binary, "OrOperator", OpCodes.Xor);
            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                dynamic instance = binaryInteractor.CreateInstance(nameSpace);
                bool actual = instance.OrOperator(true, true);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
