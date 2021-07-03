using System.IO;
using Faultify.Analyze.Analyzers;
using Faultify.Tests.UnitTests.Utils;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    internal class ConstantTests
    {
        private const string ConstantBoolTrueName = "CONSTANT_BOOL_TRUE";
        private const string ConstantBoolFalseName = "CONSTANT_BOOL_FALSE";

        private const string ConstantIntName = "CONSTANT_INT";
        private const string ConstantDoubleName = "CONSTANT_DOUBLE";
        private const string ConstantLongName = "CONSTANT_LONG";
        private const string ConstantShortName = "CONSTANT_SHORT";
        private const string ConstantFloatName = "CONSTANT_FLOAT";
        private const string ConstantUintName = "CONSTANT_UINT";
        private const string ConstantUlongName = "CONSTANT_ULONG";
        private const string ConstantUshortName = "CONSTANT_USHORT";
        private const string ConstantSbyteName = "CONSTANT_SBYTE";

        private const string ConstantByteName = "CONSTANT_BYTE";

        private const string ConstantStringName = "CONSTANT_STRING";
        private const string ConstantStringSingleCharacterName = "CONSTANT_STRING_SINGLECHARACTER";
        private const string ConstantStringTwoCharactersName = "CONSTANT_STRING_TWOCHARACTERS";
        private readonly string folder = Path.Combine("UnitTests", "TestSource", "ConstantTarget.cs");
        private readonly string nameSpace = "Faultify.Tests.UnitTests.TestSource.ConstantTarget";


        [TestCase(true, ConstantBoolTrueName)]
        [TestCase(false, ConstantBoolFalseName)]
        public void BooleanConstant_PreMutation(bool expected, string constant)
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, constant);
                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void BooleanConstant_PostMutation_TrueToFalse()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = false;

            // Act
            byte[] mutatedBinary =
                DllTestHelper.MutateField<ConstantAnalyzer>(binary, ConstantBoolTrueName, false);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantBoolTrueName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void BooleanConstant_PostMutation_FalseToTrue()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            byte[] mutatedBinary =
                DllTestHelper.MutateField<ConstantAnalyzer>(binary, ConstantBoolFalseName, true);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantBoolTrueName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void IntConstant_PreMutation_Is_1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantIntName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void IntConstant_PostMutation_IsNot_1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantIntName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantIntName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void DoubleConstant_PreMutation_Is_1dot1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1.1;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantDoubleName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void DoubleConstant_PostMutation_IsNot_1dot1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1.1;

            // Act
            byte[] mutatedBinary =
                DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantDoubleName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantDoubleName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void LongConstant_PreMutation_Is_300000()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            long expected = 300000;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantLongName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void LongConstant_PostMutation_IsNot_300000()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            long expected = 300000;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantLongName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantLongName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void ShortConstant_PreMutation_Is_1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            short expected = 1;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantShortName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void ShortConstant_PostMutation_IsNot_1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            short expected = 1;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantShortName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantShortName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void FloatConstant_PreMutation_Is_1dot12()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1.12f;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantFloatName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void FloatConstant_PostMutation_IsNot_1dot12()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1.12f;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantFloatName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantFloatName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void UintConstant_PreMutation_Is()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            uint expected = 1;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantUintName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void UintConstant_PostMutation_IsNot()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            uint expected = 1;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantUintName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantUintName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void UlongConstant_PreMutation_Is_1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            ulong expected = 1;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantUlongName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void UlongConstant_PostMutation_IsNot_1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            ulong expected = 1;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantUlongName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantUlongName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void UshortConstant_PreMutation_Is_1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            ushort expected = 1;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantUshortName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void UshortConstant_PostMutation_IsNot_1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            ulong expected = 1;

            // Act
            byte[] mutatedBinary =
                DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantUshortName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantUshortName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void SbyteConstant_PreMutation_Is_1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            sbyte expected = 1;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantSbyteName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void SbyteConstant_PostMutation_IsNot_1()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            sbyte expected = 1;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantSbyteName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantSbyteName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void ByteConstant_PreMutation_Is_240()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            byte expected = 240;

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantByteName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void ByteConstant_PostMutation_IsNot_240()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            byte expected = 240;

            // Act
            byte[] mutatedBinary = DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantByteName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantByteName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstant_PreMutation_Is_test()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "test";

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantStringName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstant_PostMutation_IsNot_test()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "test";

            // Act
            byte[] mutatedBinary =
                DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantStringName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantStringName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstantSingleCharacter_PreMutation_Is_A()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "A";

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantStringSingleCharacterName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstantSingleCharacter_PostMutation_IsNot_A()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "A";

            // Act
            byte[] mutatedBinary =
                DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantStringSingleCharacterName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantStringSingleCharacterName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstantTwoCharacters_PreMutation_Is_AB()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "AB";

            // Act
            using (DllTestHelper binaryInteractor = new DllTestHelper(binary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantStringTwoCharactersName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstantTwoCharacters_PostMutation_IsNot_AB()
        {
            // Arrange
            byte[] binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "AB";

            // Act
            byte[] mutatedBinary =
                DllTestHelper.MutateConstant<ConstantAnalyzer>(binary, ConstantStringTwoCharactersName);

            using (DllTestHelper binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                object actual = binaryInteractor.GetField(nameSpace, ConstantStringTwoCharactersName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }
    }
}
