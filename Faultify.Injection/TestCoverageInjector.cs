using System;
using System.IO;
using System.Linq;
using Faultify.TestRunner.Shared;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Faultify.Injection
{
    /// <summary>
    ///     Injects coverage code into an assembly.
    /// </summary>
    public class TestCoverageInjector
    {
        private static readonly Lazy<TestCoverageInjector> Lazy =
            new(() => new TestCoverageInjector());

        private readonly string _currentAssemblyPath = typeof(TestCoverageInjector).Assembly.Location;
        private readonly MethodDefinition _initializeMethodDefinition;
        private readonly MethodDefinition _registerTargetCoverage;
        private readonly MethodDefinition _registerTestCoverage;

        private TestCoverageInjector()
        {
            // Retrieve the method definition for the register functions. 
            var registerTargetCoverage = nameof(CoverageRegistry.RegisterTargetCoverage);
            var registerTestCoverage = nameof(CoverageRegistry.RegisterTestCoverage);
            var initializeMethodName = nameof(CoverageRegistry.Initialize);

            using var injectionAssembly = ModuleDefinition.ReadModule(_currentAssemblyPath);

            _registerTargetCoverage = injectionAssembly.Types
                .SelectMany(x => x.Methods.Where(y => y.Name == registerTargetCoverage)).First();

            _registerTestCoverage = injectionAssembly.Types
                .SelectMany(x => x.Methods.Where(y => y.Name == registerTestCoverage)).First();

            _initializeMethodDefinition = injectionAssembly.Types
                .SelectMany(x => x.Methods.Where(y => y.Name == initializeMethodName)).First();

            if (_initializeMethodDefinition == null || _registerTargetCoverage == null)
                throw new Exception("Testcoverage Injector could not initialize injection methods");
        }

        public static TestCoverageInjector Instance => Lazy.Value;

        /// <summary>
        ///     Injects a call to <see cref="CoverageRegistry" /> Initialize method into the
        ///     <Module>
        ///         initialize function.
        ///         For more info see: https://einaregilsson.com/module-initializers-in-csharp/
        /// </summary>
        /// <param name="toInjectModule"></param>
        public void InjectModuleInit(ModuleDefinition toInjectModule)
        {
            File.Copy(_currentAssemblyPath,
                Path.Combine(Path.GetDirectoryName(toInjectModule.FileName), Path.GetFileName(_currentAssemblyPath)),
                true);

            const MethodAttributes moduleInitAttributes = MethodAttributes.Static
                                                          | MethodAttributes.Assembly
                                                          | MethodAttributes.SpecialName
                                                          | MethodAttributes.RTSpecialName;

            var assembly = toInjectModule.Assembly;
            var moduleType = assembly.MainModule.GetType("<Module>");
            var method = toInjectModule.ImportReference(_initializeMethodDefinition);

            // Get or create ModuleInit method
            var cctor = moduleType.Methods.FirstOrDefault(moduleTypeMethod => moduleTypeMethod.Name == ".cctor");
            if (cctor == null)
            {
                cctor = new MethodDefinition(".cctor", moduleInitAttributes, method.ReturnType);
                moduleType.Methods.Add(cctor);
            }

            var isCallAlreadyDone = cctor.Body.Instructions.Any(instruction =>
                instruction.OpCode == OpCodes.Call && instruction.Operand == method);

            // If the method is not called, we can add it
            if (!isCallAlreadyDone)
            {
                var ilProcessor = cctor.Body.GetILProcessor();
                var retInstruction =
                    cctor.Body.Instructions.FirstOrDefault(instruction => instruction.OpCode == OpCodes.Ret);
                var callMethod = ilProcessor.Create(OpCodes.Call, method);

                if (retInstruction == null)
                {
                    // If a ret instruction is not present, add the method call and ret
                    // Insert instruction that loads the meta data token as parameter for the register method.
                    ilProcessor.Append(callMethod);
                    ilProcessor.Emit(OpCodes.Ret);
                }
                else
                {
                    // If a ret instruction is already present, just add the method to call before
                    ilProcessor.InsertBefore(retInstruction, callMethod);
                }
            }
        }

        /// <summary>
        ///     Injects the required references for the `Faultify.Injection` <see cref="CoverageRegistry" /> code into the given
        ///     module.
        /// </summary>
        /// <param name="module"></param>
        public void InjectAssemblyReferences(ModuleDefinition module)
        {
            // Find the references for `Faultify.TestRunner.Shared` and copy it over to the module directory and add it as reference.
            var assembly = typeof(MutationCoverage).Assembly;

            var dest = Path.Combine(Path.GetDirectoryName(module.FileName), Path.GetFileName(assembly.Location));
            File.Copy(assembly.Location, dest, true);

            var shared =
                _registerTargetCoverage.Module.AssemblyReferences.First(x => x.Name == assembly.GetName().Name);

            module.AssemblyReferences.Add(shared);
            module.AssemblyReferences.Add(_registerTargetCoverage.Module.Assembly.Name);
        }

        /// <summary>
        ///     Injects the coverage register function for each method in the given module.
        /// </summary>
        public void InjectTargetCoverage(ModuleDefinition module)
        {
            foreach (var typeDefinition in module.Types.Where(x => !x.Name.StartsWith("<")))
                // Find sum method
            foreach (var method in typeDefinition.Methods)
            {
                var registerMethodReference = method.Module.ImportReference(_registerTargetCoverage);

                if (method.Body == null)
                    continue;

                var processor = method.Body.GetILProcessor();

                // Insert instruction that loads the meta data token as parameter for the register method.
                var assemblyName = processor.Create(OpCodes.Ldstr, method.Module.Assembly.Name.Name);

                // Insert instruction that loads the meta data token as parameter for the register method.
                var entityHandle = processor.Create(OpCodes.Ldc_I4, method.MetadataToken.ToInt32());

                // Insert instruction that calls the register function.
                var callInstruction = processor.Create(OpCodes.Call, registerMethodReference);

                method.Body.Instructions.Insert(0, callInstruction);
                method.Body.Instructions.Insert(0, entityHandle);
                method.Body.Instructions.Insert(0, assemblyName);
            }
        }

        /// <summary>
        ///     Injects the test register function for each test method in the given module.
        /// </summary>
        public void InjectTestCoverage(ModuleDefinition module)
        {
            module.AssemblyReferences.Add(_registerTestCoverage.Module.Assembly.Name);
            module.AssemblyReferences.Add(
                _registerTargetCoverage.Module.AssemblyReferences.First(x => x.Name == "Faultify.TestRunner.Shared"));

            foreach (var typeDefinition in module.Types.Where(x => !x.Name.StartsWith("<")))
            foreach (var method in typeDefinition.Methods
                .Where(m => m.HasCustomAttributes && m.CustomAttributes
                    .Any(x => x.AttributeType.Name == "TestAttribute" ||
                              x.AttributeType.Name == "TestMethodAttribute" ||
                              x.AttributeType.Name == "FactAttribute")))
            {
                var registerMethodReference = method.Module.ImportReference(_registerTestCoverage);

                if (method.Body == null)
                    continue;

                var processor = method.Body.GetILProcessor();
                // Insert instruction that loads the meta data token as parameter for the register method.
                var entityHandle = processor.Create(OpCodes.Ldstr, method.DeclaringType.FullName + "." + method.Name);

                // Insert instruction that calls the register function.
                var callInstruction = processor.Create(OpCodes.Call, registerMethodReference);

                method.Body.Instructions.Insert(0, callInstruction);
                method.Body.Instructions.Insert(0, entityHandle);
            }
        }
    }
}