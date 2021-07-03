using System.IO;
using Faultify.Analyze.Analyzers;
using Faultify.Tests.UnitTests.Utils;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    internal class ArrayTests
    {
        private readonly string _folder = Path.Combine("UnitTests", "TestSource", "ArrayTarget.cs");
        private readonly string _nameSpace = "Faultify.Tests.UnitTests.TestSource.ArrayTarget";


        [TestCase("IntArray_Long")]
        [TestCase("UIntArray_Long")]
        [TestCase("LongArray_Long")]
        [TestCase("ULongArray_Long")]
        [TestCase("FloatArray_Long")]
        [TestCase("DoubleArray_Long")]
        [TestCase("ByteArray_Long")]
        [TestCase("SbyteArray_Long")]
        [TestCase("ShortArray_Long")]
        [TestCase("UshortArray_Long")]
        [TestCase("BoolArray_Long")]
        [TestCase("CharArray_Long")]
        public void Array_PostMutation(string methodName)
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(_folder);

            object original;
            object actual;

            // Act
            using (DllTestHelper originalInteractor = new DllTestHelper(binary))
            {
                original = originalInteractor.DynamicMethodCall(_nameSpace, methodName, null);
            }

            byte[] mutatedBinary = DllTestHelper.MutateArray<ArrayAnalyzer>(binary, methodName);
            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                actual = binaryInteractor.DynamicMethodCall(_nameSpace, methodName, null);
            }

            // Assert
            Assert.AreNotEqual(original, actual);
        }
    }
}
