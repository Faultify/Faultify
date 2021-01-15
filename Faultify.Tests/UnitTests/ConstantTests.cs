using System.IO;
using Faultify.Analyzers.ConstantAnalyzer;
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
            var binary = DllTestHelper.CompileTestBinary(folder);

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, constant);
                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void BooleanConstant_PostMutation_TrueToFalse()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = false;

            // Act
            var mutatedBinary =
                DllTestHelper.MutateField<BooleanConstantMutationAnalyzer>(binary, ConstantBoolTrueName, false);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantBoolTrueName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void BooleanConstant_PostMutation_FalseToTrue()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = true;

            // Act
            var mutatedBinary =
                DllTestHelper.MutateField<BooleanConstantMutationAnalyzer>(binary, ConstantBoolFalseName, true);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantBoolTrueName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void IntConstant_PreMutation_Is_1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantIntName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void IntConstant_PostMutation_IsNot_1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1;

            // Act
            var mutatedBinary = DllTestHelper.MutateConstant<NumberConstantMutationAnalyzer>(binary, ConstantIntName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantIntName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void DoubleConstant_PreMutation_Is_1dot1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1.1;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantDoubleName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void DoubleConstant_PostMutation_IsNot_1dot1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1.1;

            // Act
            var mutatedBinary =
                DllTestHelper.MutateConstant<NumberConstantMutationAnalyzer>(binary, ConstantDoubleName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantDoubleName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void LongConstant_PreMutation_Is_300000()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            long expected = 300000;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantLongName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void LongConstant_PostMutation_IsNot_300000()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            long expected = 300000;

            // Act
            var mutatedBinary = DllTestHelper.MutateConstant<NumberConstantMutationAnalyzer>(binary, ConstantLongName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantLongName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void ShortConstant_PreMutation_Is_1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            short expected = 1;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantShortName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void ShortConstant_PostMutation_IsNot_1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            short expected = 1;

            // Act
            var mutatedBinary = DllTestHelper.MutateConstant<NumberConstantMutationAnalyzer>(binary, ConstantShortName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantShortName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void FloatConstant_PreMutation_Is_1dot12()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1.12f;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantFloatName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void FloatConstant_PostMutation_IsNot_1dot12()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = 1.12f;

            // Act
            var mutatedBinary = DllTestHelper.MutateConstant<NumberConstantMutationAnalyzer>(binary, ConstantFloatName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantFloatName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void UintConstant_PreMutation_Is()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            uint expected = 1;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantUintName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void UintConstant_PostMutation_IsNot()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            uint expected = 1;

            // Act
            var mutatedBinary = DllTestHelper.MutateConstant<NumberConstantMutationAnalyzer>(binary, ConstantUintName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantUintName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void UlongConstant_PreMutation_Is_1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            ulong expected = 1;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantUlongName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void UlongConstant_PostMutation_IsNot_1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            ulong expected = 1;

            // Act
            var mutatedBinary = DllTestHelper.MutateConstant<NumberConstantMutationAnalyzer>(binary, ConstantUlongName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantUlongName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void UshortConstant_PreMutation_Is_1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            ushort expected = 1;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantUshortName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void UshortConstant_PostMutation_IsNot_1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            ulong expected = 1;

            // Act
            var mutatedBinary =
                DllTestHelper.MutateConstant<NumberConstantMutationAnalyzer>(binary, ConstantUshortName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantUshortName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void SbyteConstant_PreMutation_Is_1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            sbyte expected = 1;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantSbyteName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void SbyteConstant_PostMutation_IsNot_1()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            sbyte expected = 1;

            // Act
            var mutatedBinary = DllTestHelper.MutateConstant<NumberConstantMutationAnalyzer>(binary, ConstantSbyteName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantSbyteName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void ByteConstant_PreMutation_Is_240()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            byte expected = 240;

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantByteName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void ByteConstant_PostMutation_IsNot_240()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            byte expected = 240;

            // Act
            var mutatedBinary = DllTestHelper.MutateConstant<NumberConstantMutationAnalyzer>(binary, ConstantByteName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantByteName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstant_PreMutation_Is_test()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "test";

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantStringName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstant_PostMutation_IsNot_test()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "test";

            // Act
            var mutatedBinary =
                DllTestHelper.MutateConstant<StringConstantMutationAnalyzer>(binary, ConstantStringName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantStringName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstantSingleCharacter_PreMutation_Is_A()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "A";

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantStringSingleCharacterName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstantSingleCharacter_PostMutation_IsNot_A()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "A";

            // Act
            var mutatedBinary =
                DllTestHelper.MutateConstant<StringConstantMutationAnalyzer>(binary, ConstantStringSingleCharacterName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantStringSingleCharacterName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstantTwoCharacters_PreMutation_Is_AB()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "AB";

            // Act
            using (var binaryInteractor = new DllTestHelper(binary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantStringTwoCharactersName);

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void StringConstantTwoCharacters_PostMutation_IsNot_AB()
        {
            // Arrange
            var binary = DllTestHelper.CompileTestBinary(folder);
            var expected = "AB";

            // Act
            var mutatedBinary =
                DllTestHelper.MutateConstant<StringConstantMutationAnalyzer>(binary, ConstantStringTwoCharactersName);

            using (var binaryInteractor = new DllTestHelper(mutatedBinary))
            {
                var actual = binaryInteractor.GetField(nameSpace, ConstantStringTwoCharactersName);

                // Assert
                Assert.AreNotEqual(expected, actual);
            }
        }
    }
}