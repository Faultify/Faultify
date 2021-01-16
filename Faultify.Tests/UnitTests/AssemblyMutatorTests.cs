using System.IO;
using System.Linq;
using Faultify.Analyzers;
using Faultify.Analyzers.AssemblyMutator;
using Faultify.Analyzers.ConstantAnalyzer;
using Faultify.Analyzers.OpcodeAnalyzer;
using Faultify.Tests.UnitTests.Utils;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests
{
    public class AssemblyMutatorTests
    {
        private readonly string _folder = Path.Combine("UnitTests", "TestSource", "TestAssemblyTarget.cs");

        private readonly string _nameSpaceTestAssemblyTarget1 =
            "Faultify.Tests.UnitTests.TestSource.TestAssemblyTarget1";

        private readonly string _nameSpaceTestAssemblyTarget2 =
            "Faultify.Tests.UnitTests.TestSource.TestAssemblyTarget2";

        [SetUp]
        public void LoadTestAssembly()
        {
            var binary = DllTestHelper.CompileTestBinary(_folder);
            File.WriteAllBytes("test.dll", binary);
        }

        [TearDown]
        public void RemoveTestAssembly()
        {
            File.Delete("test.dll");
        }

        [Test]
        public void AssemblyMutator_Has_Right_Types()
        {
            using Stream stream = new MemoryStream(File.ReadAllBytes("test.dll"));
            using var mutator = new AssemblyMutator(stream);

            Assert.AreEqual(mutator.Types.Count, 2);
            Assert.AreEqual(mutator.Types[0].AssemblyQualifiedName, _nameSpaceTestAssemblyTarget1);
            Assert.AreEqual(mutator.Types[1].AssemblyQualifiedName, _nameSpaceTestAssemblyTarget2);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget1_Has_Right_Methods()
        {
            using Stream stream = new MemoryStream(File.ReadAllBytes("test.dll"));
            using var mutator = new AssemblyMutator(stream);
            var target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget1);

            Assert.AreEqual(target1.Methods.Count, 3);
            Assert.IsNotNull(target1.Methods.FirstOrDefault(x => x.Name == "TestMethod1"), null);
            Assert.IsNotNull(target1.Methods.FirstOrDefault(x => x.Name == "TestMethod2"), null);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget2_Has_Right_Methods()
        {
            using Stream stream = new MemoryStream(File.ReadAllBytes("test.dll"));
            using var mutator = new AssemblyMutator(stream);
            var target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget2);

            Assert.AreEqual(target1.Methods.Count, 4);
            Assert.IsNotNull(target1.Methods.FirstOrDefault(x => x.Name == "TestMethod1"), null);
            Assert.IsNotNull(target1.Methods.FirstOrDefault(x => x.Name == "TestMethod2"), null);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget1_Has_Right_Fields()
        {
            using Stream stream = new MemoryStream(File.ReadAllBytes("test.dll"));
            using var mutator = new AssemblyMutator(stream);
            var target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget1);

            Assert.AreEqual(target1.Fields.Count, 2); // ctor, cctor, two target methods.
            Assert.IsNotNull(target1.Fields.FirstOrDefault(x => x.Name == "Constant"), null);
            Assert.IsNotNull(target1.Fields.FirstOrDefault(x => x.Name == "Static"), null);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget2_Has_Right_Fields()
        {
            using Stream stream = new MemoryStream(File.ReadAllBytes("test.dll"));
            using var mutator = new AssemblyMutator(stream);
            var target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget2);

            Assert.AreEqual(target1.Fields.Count, 2); // ctor, cctor, two target methods.
            Assert.IsNotNull(target1.Fields.FirstOrDefault(x => x.Name == "Constant"), null);
            Assert.IsNotNull(target1.Fields.FirstOrDefault(x => x.Name == "Static"), null);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget1_TestMethod1_Has_Right_Mutations()
        {
            using Stream stream = new MemoryStream(File.ReadAllBytes("test.dll"));
            using var mutator = new AssemblyMutator(stream);
            var target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget1);
            var method1 = target1.Methods.FirstOrDefault(x => x.Name == "TestMethod1");
            var mutations = method1.OpCodeMutations(MutationLevel.Detailed).Select(x => x).ToList();

            var arithmeticMutations =
                mutations.FirstOrDefault(x => x.AnalyzerName == new ArithmeticMutationAnalyzer().Name);
            var comparisonMutations =
                mutations.FirstOrDefault(x => x.AnalyzerName == new ComparisonMutationAnalyzer().Name);

            Assert.AreEqual(mutations.Count, 2);
            Assert.IsNotNull(arithmeticMutations, null);
            Assert.IsNotNull(comparisonMutations, null);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget1_Constant_Has_Right_Mutation()
        {
            using Stream stream = new MemoryStream(File.ReadAllBytes("test.dll"));
            using var mutator = new AssemblyMutator(stream);
            var target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget1);
            var field = target1.Fields.FirstOrDefault(x => x.Name == "Constant");
            var mutations = field.ConstantFieldMutations(MutationLevel.Detailed);

            var arithmeticMutations =
                mutations.FirstOrDefault(x => x.AnalyzerName == new BooleanConstantMutationAnalyzer().Name);

            Assert.AreEqual(mutations.Count(), 1);
            Assert.IsNotNull(arithmeticMutations, null);
        }
    }
}