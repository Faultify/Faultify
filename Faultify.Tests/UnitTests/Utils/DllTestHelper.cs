extern alias MC;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Faultify.Analyze;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
using MC::Mono.Cecil;
using MC::Mono.Cecil.Cil;
using MC::Mono.Cecil.Rocks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using NUnit.Framework;

namespace Faultify.Tests.UnitTests.Utils
{
    public class DllTestHelper : AssemblyLoadContext, IDisposable
    {
        private readonly DllTestHelper _afterMutationContext;
        private readonly Assembly _assembly;

        /// <summary>
        ///     Code is converted to a Binary stream so it can be read
        /// </summary>
        /// <param name="binary"></param>
        public DllTestHelper(byte[] binary)
        {
            using MemoryStream memoryStream = new MemoryStream(binary, false);
            _afterMutationContext = new DllTestHelper();
            _assembly = _afterMutationContext.LoadFromStream(memoryStream);
        }

        /// <summary>
        ///     Creates an assembly context to trow away code after it has been used
        /// </summary>
        public DllTestHelper() : base(true) { }

        /// <summary>
        ///     Disposes the assembly context
        /// </summary>
        public void Dispose()
        {
            _afterMutationContext.Unload();
        }

        /// <summary>
        ///     Compiles CS code to binary code (how Dll's supposed to look)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] CompileTestBinary(string path)
        {
            string source = File.ReadAllText(path);

            CSharpCompilation compilation = CSharpCompilation.Create("test.dll",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            MemoryStream memoryStream = new MemoryStream();
            EmitResult emitResult = compilation.Emit(memoryStream);

            if (!emitResult.Success)
            {
                Assert.Fail("Could not compile Test Target");
            }

            return memoryStream.ToArray();
        }


        /// <summary>
        ///     Execute a mutation and return the mutated binary.
        /// </summary>
        /// <typeparam name="TMutator"></typeparam>
        /// <param name="binary"></param>
        /// <param name="method"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        public static byte[] MutateMethod<TMutator>(
            byte[] binary,
            string method,
            OpCode expected,
            bool simplefy = false
        ) where TMutator : IAnalyzer<OpCodeMutation, Instruction>
        {
            ModuleDefinition module = ModuleDefinition.ReadModule(new MemoryStream(binary, false));
            MethodDefinition mutateMethod =
                module.Types.SelectMany(x => x.Methods).FirstOrDefault(x => x.Name == method);
            TMutator mutator = Activator.CreateInstance<TMutator>();

            if (simplefy)
            {
                mutateMethod.Body.SimplifyMacros();
            }

            foreach (Instruction instruction in mutateMethod.Body.Instructions)
            {
                IMutationGroup<OpCodeMutation> possibleOperatorMutations =
                    mutator.GenerateMutations(instruction, MutationLevel.Detailed);

                foreach (OpCodeMutation mutation in possibleOperatorMutations)
                {
                    mutation.Mutate();

                    if (mutation.Replacement == expected)
                    {
                        MemoryStream mutatedBinaryStream = new MemoryStream();
                        module.Write(mutatedBinaryStream);
                        File.WriteAllBytes("debug.dll", mutatedBinaryStream.ToArray());
                        return mutatedBinaryStream.ToArray();
                    }
                }
            }

            Assert.Fail();
            return new byte[] { };
        }

        /// <summary>
        ///     Execute a mutation and return the mutated binary.
        /// </summary>
        /// <typeparam name="TMutator"></typeparam>
        /// <param name="binary"></param>
        /// <param name="method"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        public static byte[] MutateMethodVariables<TMutator>(byte[] binary, string method, bool simplify = false)
            where TMutator : IAnalyzer<VariableMutation, MethodDefinition>
        {
            ModuleDefinition module = ModuleDefinition.ReadModule(new MemoryStream(binary, false));
            MethodDefinition mutateMethod =
                module.Types.SelectMany(x => x.Methods).FirstOrDefault(x => x.Name == method);
            TMutator mutator = Activator.CreateInstance<TMutator>();

            if (simplify)
            {
                mutateMethod.Body.SimplifyMacros();
            }

            IMutationGroup<VariableMutation> possibleOperatorMutations =
                mutator.GenerateMutations(mutateMethod, MutationLevel.Detailed);

            foreach (VariableMutation mutation in possibleOperatorMutations)
            {
                mutation.Mutate();

                MemoryStream mutatedBinaryStream = new MemoryStream();
                module.Write(mutatedBinaryStream);
                File.WriteAllBytes("debug.dll", mutatedBinaryStream.ToArray());
                return mutatedBinaryStream.ToArray();
            }

            Assert.Fail();
            return new byte[] { };
        }

        public static byte[] MutateField<TMutator>(byte[] binary, string fieldName, object expected)
            where TMutator : IAnalyzer<ConstantMutation, FieldDefinition>
        {
            ModuleDefinition module = ModuleDefinition.ReadModule(new MemoryStream(binary, false));

            FieldDefinition field = module.Types
                .SelectMany(x => x.Fields)
                .FirstOrDefault(x => x.Name == fieldName);

            TMutator mutator = Activator.CreateInstance<TMutator>();

            IMutationGroup<ConstantMutation> possibleOperatorMutations =
                mutator.GenerateMutations(field, MutationLevel.Detailed);

            foreach (ConstantMutation mutation in possibleOperatorMutations)
            {
                mutation.Mutate();

                if (mutation.Replacement.Equals(expected))
                {
                    MemoryStream mutatedBinaryStream = new MemoryStream();
                    module.Write(mutatedBinaryStream);
                    return mutatedBinaryStream.ToArray();
                }
            }

            Assert.Fail();
            return new byte[] { };
        }

        public static byte[] MutateConstant<TMutator>(byte[] binary, string fieldName)
            where TMutator : IAnalyzer<ConstantMutation, FieldDefinition>
        {
            ModuleDefinition module = ModuleDefinition.ReadModule(new MemoryStream(binary, false));

            FieldDefinition field = module.Types
                .SelectMany(x => x.Fields)
                .FirstOrDefault(x => x.Name == fieldName);

            TMutator mutator = Activator.CreateInstance<TMutator>();
            ConstantMutation mutation = mutator.GenerateMutations(field, MutationLevel.Detailed).First();
            mutation.Mutate();
            MemoryStream mutatedBinaryStream = new MemoryStream();
            module.Write(mutatedBinaryStream);
            return mutatedBinaryStream.ToArray();
        }

        public static byte[] MutateArray<TMutator>(byte[] binary, string methodName)
            where TMutator : IAnalyzer<ArrayMutation, MethodDefinition>
        {
            ModuleDefinition module = ModuleDefinition.ReadModule(new MemoryStream(binary, false));

            MethodDefinition method = module.Types
                .SelectMany(x => x.Methods)
                .FirstOrDefault(x => x.Name == methodName);

            TMutator mutator = Activator.CreateInstance<TMutator>();
            ArrayMutation mutation = mutator.GenerateMutations(method, MutationLevel.Detailed).First();

            mutation.Mutate();
            MemoryStream mutatedBinaryStream = new MemoryStream();
            module.Write(mutatedBinaryStream);
            File.WriteAllBytes("debug.dll", mutatedBinaryStream.ToArray());
            return mutatedBinaryStream.ToArray();
        }

        public object DynamicMethodCall(string name, string methodName, object[] parameters)
        {
            dynamic instance = CreateInstance(name);
            MethodInfo? method = ((object) instance).GetType().GetMethod(methodName.FirstCharToUpper());
            return method.Invoke(instance, parameters);
        }

        /// <summary>
        ///     Creates and returns a type
        /// </summary>
        public dynamic CreateInstance(string assemblyQualifiedName)
        {
            Type? type = _assembly.GetType(assemblyQualifiedName);
            return Activator.CreateInstance(type);
        }

        /// <summary>
        ///     Creates and returns a type
        /// </summary>
        public object GetField(string assemblyQualifiedName, string field)
        {
            Type? type = _assembly.GetType(assemblyQualifiedName);
            return type.GetField(field).GetRawConstantValue();
        }

        /// <summary>
        ///     Loads an assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
