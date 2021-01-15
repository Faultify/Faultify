using System.IO;
using Faultify.Analyzers.OpcodeAnalyzer;
using Faultify.Tests.UnitTests.Utils;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    internal class BitwiseTests
    {
        private readonly string _folder = Path.Combine("UnitTests", "TestSource", "BitwiseTarget.cs");
        private readonly string _nameSpace = "Faultify.Tests.UnitTests.TestSource.BitwiseTarget";

        [TestCase("AndOperator", (uint) 0b_1111_1000, (uint) 0b_1001_1101, (uint) 0b_1001_1000)]
        [TestCase("OrOperator", (uint) 0b_1111_1000, (uint) 0b_1001_1101, (uint) 0b_1111_1101)]
        [TestCase("XorOperator", (uint) 0b_1111_1000, (uint) 0b_1001_1101, (uint) 0b_0110_0101)]
        public void Bitwise_PreMutation(string methodName, object argument1, object argument2, uint expected)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = (uint) binaryInteractor.DynamicMethodCall(_nameSpace, methodName.FirstCharToUpper(),
                    new[] {argument1, argument2});

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("AndOperator", (uint) 0b_1111_1000, (uint) 0b_1001_1101, nameof(OpCodes.Or), (uint) 0b_1111_1101)]
        [TestCase("AndOperator", (uint) 0b_1111_1000, (uint) 0b_1001_1101, nameof(OpCodes.Xor), (uint) 0b_0110_0101)]
        [TestCase("OrOperator", (uint) 0b_1111_1000, (uint) 0b_1001_1101, nameof(OpCodes.And), (uint) 0b_1001_1000)]
        [TestCase("OrOperator", (uint) 0b_1111_1000, (uint) 0b_1001_1101, nameof(OpCodes.Xor), (uint) 0b_0110_0101)]
        [TestCase("XorOperator", (uint) 0b_1111_1000, (uint) 0b_1001_1101, nameof(OpCodes.Or), (uint) 0b_1111_1101)]
        [TestCase("XorOperator", (uint) 0b_1111_1000, (uint) 0b_1001_1101, nameof(OpCodes.And), (uint) 0b_1001_1000)]
        public void Bitwise_PostMutation(string methodName, object argument1, object argument2,
            string expectedOpCodeName, uint expected)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);
            var opCodeExpected = expectedOpCodeName.ParseOpCode();

            // Act
            var mutatedBinary =
                DllTestHelper.MutateMethod<BitwiseMutationAnalyzer>(binary, methodName, opCodeExpected);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = (uint) binaryInteractor.DynamicMethodCall(_nameSpace, methodName.FirstCharToUpper(),
                    new[] {argument1, argument2});

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }
    }
}