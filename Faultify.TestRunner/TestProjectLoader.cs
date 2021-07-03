extern alias MC;
using System;
using System.Collections.Generic;
using Faultify.Analyze.AssemblyMutator;
using MC::Mono.Cecil;

namespace Faultify.TestRunner
{
    public class TestProjectInfo : IDisposable
    {
        public TestProjectInfo(TestFramework testFramework, ModuleDefinition testModule)
        {
            TestFramework = testFramework;
            TestModule = testModule;
        }
        public TestFramework TestFramework { get; set; }
        public ModuleDefinition TestModule { get; set; }
        public List<AssemblyMutator> DependencyAssemblies { get; set; } = new List<AssemblyMutator>();

        /// <summary>
        /// <inheritdoc cref="IDisposable"/>
        /// </summary>
        public void Dispose()
        {
            foreach (var assemblyMutator in DependencyAssemblies) assemblyMutator.Dispose();
            DependencyAssemblies.Clear();
            TestModule!.Dispose();
        }
    }
}
