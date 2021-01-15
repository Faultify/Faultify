using System;
using System.IO;
using Faultify.Core.ProjectAnalyzing;
using Faultify.Tests.UnitTests.Utils;
using Mono.Cecil;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    internal class DecompilerTests
    {
        private const string TypeName = "DecompilerTestTarget";
        private readonly string _folder = Path.Combine("UnitTests", "TestSource", "DecompilerTestTarget.cs");

        private ModuleDefinition _module;

        private Stream _stream;

        [SetUp]
        public void LoadTestAssembly()
        {
            var binary = DllTestHelper.CompileTestBinary(_folder);
            File.WriteAllBytes("test.dll", binary);
            _stream = new MemoryStream(File.ReadAllBytes("test.dll"));
            _module = ModuleDefinition.ReadModule(_stream);
            _stream.Position = 0;
        }

        [TearDown]
        public void RemoveTestAssembly()
        {
            File.Delete("test.dll");
            _stream.Dispose();
            _module.Dispose();
        }

        [TestCase(
            "public class DecompilerTestTarget{private const bool _testBool = true;public int TestReturnInt(int test){return test;}public int TestReturnInt(int test, int test2){return test + test2;}}")]
        public void Decompile_Test_Type(string expectedNotClean)
        {
            // Arrange
            var cd = new CodeDecompiler("test.dll", _stream);
            var handle = DecompileHandleHelper.DecompileType(_module, TypeName);
            var expected = expectedNotClean.CleanUpCode();

            // Act
            var actual = cd.Decompile(handle).CleanUpCode();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase("TestReturnInt", "public int TestReturnInt(int test){return test;}",
            typeof(int))]
        [TestCase("TestReturnInt",
            "public int TestReturnInt(int test, int test2){return test + test2;}", typeof(int), typeof(int))]
        public void Decompile_Test_method(string methodName, string expectedNotClean,
            params Type[] typeList)
        {
            // Arrange
            var cd = new CodeDecompiler("test.dll", _stream);
            var handle = DecompileHandleHelper.DecompileMethod(_module, TypeName, methodName, typeList);
            var expected = expectedNotClean.CleanUpCode();

            // Act
            var actual = cd.Decompile(handle).CleanUpCode();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase("_testBool", "private const bool _testBool = true;")]
        public void Decompile_Test_Field(string fieldName, string expectedNotClean)
        {
            // Arrange
            var cd = new CodeDecompiler("test.dll", _stream);
            var handle = DecompileHandleHelper.DecompileField(_module, TypeName, fieldName);
            var expected = expectedNotClean.CleanUpCode();

            // Act
            var actual = cd.Decompile(handle).CleanUpCode();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}