using System.IO;
using Faultify.Analyze.Analyzers;
using Faultify.Tests.UnitTests.Utils;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    internal class VariableLiteralMutatorTests
    {
        private readonly string _folder = Path.Combine("UnitTests", "TestSource", "BooleanLiteralTarget.cs");
        private readonly string _nameSpace = "Faultify.Tests.UnitTests.TestSource.BooleanLiteralTarget";

        [TestCase("True", false, true)]
        [TestCase("False", true, true)]
        public void Boolean_Variable_PostMutation(string methodName, bool expected, bool simplify)
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(_folder);

            // Act
            var mutatedBinary =
                DllTestHelper.MutateMethodVariables<VariableAnalyzer>(binary, methodName, simplify);
            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual =
                    (bool) binaryInteractor.DynamicMethodCall(_nameSpace, methodName.FirstCharToUpper(),
                        new object[] { });

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }
    }
}