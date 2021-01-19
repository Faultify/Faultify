using System.IO;
using Faultify.Analyze.OpcodeAnalyzer;
using Faultify.Tests.UnitTests.Utils;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    internal class EqualityTests
    {
        private readonly string _folder = Path.Combine("UnitTests", "TestSource", "EqualityTarget.cs");
        private readonly string _nameSpace = "Faultify.Tests.UnitTests.TestSource.EqualityTarget";

        [TestCase("MoreThanOrdered", 1, 3)]
        [TestCase("MoreThanUnOrdered", (uint) 1, (uint) 3)]
        [TestCase("LessThanOrdered", 3, 1)]
        [TestCase("LessThanUnOrdered", (uint) 3, (uint) 1)]
        [TestCase("MoreThanEqualOrdered", 1, 3)]
        [TestCase("MoreThanEqualUnOrdered", (uint) 1, (uint) 3)]
        [TestCase("NotEqualOrdered", 1, 1)]
        [TestCase("EqualOrdered", 1, 3)]
        public void Equality_PreMutation_False(string methodName, object argument1, object argument2)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);
            var expected = false;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = (bool) binaryInteractor.DynamicMethodCall(_nameSpace, methodName.FirstCharToUpper(),
                    new[] {argument1, argument2});

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("MoreThanOrdered", 3, 1)]
        [TestCase("MoreThanUnOrdered", (uint) 3, (uint) 1)]
        [TestCase("LessThanOrdered", 1, 3)]
        [TestCase("LessThanUnOrdered", (uint) 1, (uint) 3)]
        [TestCase("MoreThanEqualOrdered", 3, 1)]
        [TestCase("MoreThanEqualOrdered", 1, 1)]
        [TestCase("MoreThanEqualUnOrdered", (uint) 3, (uint) 1)]
        [TestCase("MoreThanEqualUnOrdered", (uint) 1, (uint) 1)]
        [TestCase("NotEqualOrdered", 1, 3)]
        [TestCase("EqualOrdered", 1, 1)]
        public void Equality_PreMutation_True(string methodName, object argument1, object argument2)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);
            var expected = true;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = (bool) binaryInteractor.DynamicMethodCall(_nameSpace, methodName.FirstCharToUpper(),
                    new[] {argument1, argument2});

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("MoreThanOrdered", nameof(OpCodes.Clt), 3, 1)]
        [TestCase("MoreThanUnOrdered", nameof(OpCodes.Clt_Un), (uint) 3, (uint) 1)]
        [TestCase("LessThanOrdered", nameof(OpCodes.Cgt), 1, 3)]
        [TestCase("LessThanUnOrdered", nameof(OpCodes.Cgt_Un), (uint) 1, (uint) 3)]
        [TestCase("EqualOrdered", nameof(OpCodes.Clt), 1, 1)]
        public void Equality_PostMutation_Conditional(string methodName, string expectedOpCodeName, object argument1,
            object argument2)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);
            var expected = false;
            var opCodeExpected = expectedOpCodeName.ParseOpCode();

            // Act
            var mutatedBinary =
                DllTestHelper.MutateMethod<ComparisonMutationAnalyzer>(binary, methodName, opCodeExpected);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = (bool) binaryInteractor.DynamicMethodCall(_nameSpace, methodName.FirstCharToUpper(),
                    new[] {argument1, argument2});

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("EqualOrdered", nameof(OpCodes.Beq), nameof(OpCodes.Bne_Un), 1, 1)]
        [TestCase("MoreThanEqualOrdered", nameof(OpCodes.Bge), nameof(OpCodes.Blt), 1, 1)]
        [TestCase("MoreThanEqualOrdered", nameof(OpCodes.Bge), nameof(OpCodes.Blt), 2, 1)]
        [TestCase("MoreThanEqualUnOrdered", nameof(OpCodes.Bge), nameof(OpCodes.Blt), (uint) 1, (uint) 1)]
        [TestCase("MoreThanEqualUnOrdered", nameof(OpCodes.Bge), nameof(OpCodes.Blt), (uint) 2, (uint) 1)]
        [TestCase("MoreThanOrdered", nameof(OpCodes.Bgt), nameof(OpCodes.Ble), 3, 1)]
        [TestCase("MoreThanUnOrdered", nameof(OpCodes.Bgt_Un), nameof(OpCodes.Ble_Un), (uint) 3, (uint) 1)]
        [TestCase("LessThanEqualOrdered", nameof(OpCodes.Ble), nameof(OpCodes.Bgt), 1, 3)]
        [TestCase("LessThanEqualUnOrdered", nameof(OpCodes.Ble_Un), nameof(OpCodes.Bgt_Un), (uint) 1, (uint) 3)]
        [TestCase("LessThanOrdered", nameof(OpCodes.Blt), nameof(OpCodes.Bge), 1, 3)]
        [TestCase("LessThanUnOrdered", nameof(OpCodes.Blt_Un), nameof(OpCodes.Bge_Un), (uint) 1, (uint) 3)]
        [TestCase("NotEqualOrdered", nameof(OpCodes.Bne_Un), nameof(OpCodes.Beq), 1, 3)]
        public void Equality_PostMutation_Branch(string methodName, string defaultOpCodeName, string expectedOpCodeName,
            object argument1, object argument2)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);
            var expected = false;
            var opCodeDefault = defaultOpCodeName.ParseOpCode();
            var opCodeExpected = expectedOpCodeName.ParseOpCode();

            // Act
            binary = Utils.Utils.ChangeComparisonToBranchOperator(binary, methodName, opCodeDefault);
            var mutatedBinary =
                DllTestHelper.MutateMethod<ComparisonMutationAnalyzer>(binary, methodName, opCodeExpected);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var instance = binaryInteractor.CreateInstance(_nameSpace);
                var method = ((object) instance).GetType().GetMethod(methodName.FirstCharToUpper());
                var actual = (bool) method.Invoke(instance, new[] {argument1, argument2});

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }
    }
}