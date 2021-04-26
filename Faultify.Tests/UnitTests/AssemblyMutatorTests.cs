using System.Collections.Generic;
using System.IO;
using System.Linq;
using Faultify.Analyze;
using Faultify.Analyze.Analyzers;
using Faultify.Analyze.AssemblyMutator;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
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
            byte[] binary = DllTestHelper.CompileTestBinary(_folder);
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
            using AssemblyMutator mutator = new AssemblyMutator("test.dll");

            Assert.AreEqual(mutator.Types.Count, 2);
            Assert.AreEqual(mutator.Types[0].AssemblyQualifiedName, _nameSpaceTestAssemblyTarget1);
            Assert.AreEqual(mutator.Types[1].AssemblyQualifiedName, _nameSpaceTestAssemblyTarget2);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget1_Has_Right_Methods()
        {
            using AssemblyMutator mutator = new AssemblyMutator("test.dll");
            TypeScope target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget1);

            Assert.AreEqual(target1.Methods.Count, 3);
            Assert.IsNotNull(target1.Methods.FirstOrDefault(x => x.Name == "TestMethod1"), null);
            Assert.IsNotNull(target1.Methods.FirstOrDefault(x => x.Name == "TestMethod2"), null);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget2_Has_Right_Methods()
        {
            using AssemblyMutator mutator = new AssemblyMutator("test.dll");
            TypeScope target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget2);

            Assert.AreEqual(target1.Methods.Count, 4);
            Assert.IsNotNull(target1.Methods.FirstOrDefault(x => x.Name == "TestMethod1"), null);
            Assert.IsNotNull(target1.Methods.FirstOrDefault(x => x.Name == "TestMethod2"), null);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget1_Has_Right_Fields()
        {
            using AssemblyMutator mutator = new AssemblyMutator("test.dll");
            TypeScope target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget1);

            Assert.AreEqual(target1.Fields.Count, 2); // ctor, cctor, two target methods.
            Assert.IsNotNull(target1.Fields.FirstOrDefault(x => x.Name == "Constant"), null);
            Assert.IsNotNull(target1.Fields.FirstOrDefault(x => x.Name == "Static"), null);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget2_Has_Right_Fields()
        {
            using AssemblyMutator mutator = new AssemblyMutator("test.dll");
            TypeScope target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget2);

            Assert.AreEqual(target1.Fields.Count, 2); // ctor, cctor, two target methods.
            Assert.IsNotNull(target1.Fields.FirstOrDefault(x => x.Name == "Constant"), null);
            Assert.IsNotNull(target1.Fields.FirstOrDefault(x => x.Name == "Static"), null);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget1_TestMethod1_Has_Right_Mutations()
        {
            using AssemblyMutator mutator = new AssemblyMutator("test.dll");
            TypeScope target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget1);
            MethodScope method1 = target1.Methods.FirstOrDefault(x => x.Name == "TestMethod1");
            List<IMutationGroup<OpCodeMutation>> mutations =
                method1.OpCodeMutations(MutationLevel.Detailed).Select(x => x).ToList();

            IMutationGroup<OpCodeMutation> arithmeticMutations =
                mutations.FirstOrDefault(x => x.Name == new ArithmeticAnalyzer().Name);
            IMutationGroup<OpCodeMutation> comparisonMutations =
                mutations.FirstOrDefault(x => x.Name == new ComparisonAnalyzer().Name);

            Assert.AreEqual(mutations.Count, 2);
            Assert.IsNotNull(arithmeticMutations, null);
            Assert.IsNotNull(comparisonMutations, null);
        }

        [Test]
        public void AssemblyMutator_Type_TestAssemblyTarget1_Constant_Has_Right_Mutation()
        {
            using AssemblyMutator mutator = new AssemblyMutator("test.dll");
            TypeScope target1 = mutator.Types.First(x =>
                x.AssemblyQualifiedName == _nameSpaceTestAssemblyTarget1);
            FieldScope field = target1.Fields.FirstOrDefault(x => x.Name == "Constant");
            IEnumerable<IMutationGroup<ConstantMutation>> mutations =
                field.ConstantFieldMutations(MutationLevel.Detailed);

            IMutationGroup<ConstantMutation> arithmeticMutations =
                mutations.FirstOrDefault(x => x.Name == new ConstantAnalyzer().Name);

            Assert.AreEqual(mutations.Count(), 1);
            Assert.IsNotNull(arithmeticMutations, null);
        }
    }
}
