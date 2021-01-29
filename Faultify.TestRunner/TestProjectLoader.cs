using System;
using System.Collections.Generic;
using Faultify.Analyze.AssemblyMutator;
using Mono.Cecil;

namespace Faultify.TestRunner
{
    public class TestProjectInfo : IDisposable
    {
        public ModuleDefinition TestModule { get; set; }
        public List<AssemblyMutator> DependencyAssemblies { get; set; } = new List<AssemblyMutator>();

        public void Dispose()
        {
            foreach (var assemblyMutator in DependencyAssemblies) assemblyMutator.Dispose();
            DependencyAssemblies.Clear();
            TestModule.Dispose();
        }
    }
}