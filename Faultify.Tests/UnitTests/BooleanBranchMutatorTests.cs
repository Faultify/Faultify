using System.IO;
using Faultify.Analyze;
using Faultify.Tests.UnitTests.Utils;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    internal class BooleanBranchMutatorTests
    {
        private readonly string _folder = Path.Combine("UnitTests", "TestSource", "BooleanTarget.cs");
        private readonly string _nameSpace = "Faultify.Tests.UnitTests.TestSource.BooleanTarget";


        [TestCase("BrTrue", true)]
        [TestCase("BrFalse", true)]
        public void BooleanBranch_PreMutation(string methodName, object expectedReturn)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);
            var expected = true;

            // Act

            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = (bool)binaryInteractor.DynamicMethodCall(_nameSpace, methodName.FirstCharToUpper(),
                    new[] { expectedReturn });

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("BrTrue", nameof(OpCodes.Brfalse_S), true, false)]
        [TestCase("BrFalse", nameof(OpCodes.Brtrue_S), true, false)]
        [TestCase("BrTrue", nameof(OpCodes.Brfalse), true, true)]
        [TestCase("BrFalse", nameof(OpCodes.Brtrue), true, true)]
        public void BooleanBranch_PostMutation(string methodName, string expectedOpCodeName, object argument1,
            bool simplify)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);
            var expected = false;
            var opCodeExpected = expectedOpCodeName.ParseOpCode();

            // Act
            var mutatedBinary =
                DllTestHelper.MutateMethod<BooleanBranchMutationAnalyzer>(binary, methodName, opCodeExpected, simplify);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual =
                    (bool)binaryInteractor.DynamicMethodCall(_nameSpace, methodName.FirstCharToUpper(),
                        new[] { argument1 });

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }
    }
}