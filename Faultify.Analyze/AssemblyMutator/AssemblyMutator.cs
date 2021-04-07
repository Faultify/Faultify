using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Faultify.Analyze.Analyzers;
using Faultify.Analyze.ConstantAnalyzer;
using Faultify.Analyze.Mutation;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Faultify.Analyze.AssemblyMutator
{
    /// <summary>
    ///     The `AssemblyMutator` can be used to analyze all kinds of mutations in a target assembly.
    ///     It can be extended with custom analyzers.
    ///     Though an extension must correspond to one of the following collections in `AssemblyMutator`:
    ///     <br /><br />
    ///     - ArrayMutationAnalyzers(<see cref="ArrayAnalyzer" />)<br />
    ///     - ConstantAnalyzers(<see cref="VariableAnalyzer" />)<br />
    ///     - VariableMutationAnalyzer(<see cref="Analyzers.ConstantAnalyzer" />)<br />
    ///     - OpCodeMutationAnalyzer(<see cref="OpCodeAnalyzer" />)<br />
    ///     <br /><br />
    ///     If you add your analyzer to one of those collections then it will be used in the process of analyzing.
    ///     Unfortunately, if your analyzer does not fit the interfaces, it can not be used with the `AssemblyMutator`.
    /// </summary>
    public class AssemblyMutator : IDisposable
    {
        /// <summary>
        ///     Analyzers that search for possible array mutations inside a method definition.
        /// </summary>
        public HashSet<IAnalyzer<ArrayMutation, MethodDefinition>> ArrayMutationAnalyzers =
            new HashSet<IAnalyzer<ArrayMutation, MethodDefinition>>
            {
                new ArrayAnalyzer()
            };

        /// <summary>
        ///     Analyzers that search for possible constant mutations.
        /// </summary>
        public HashSet<IAnalyzer<ConstantMutation, FieldDefinition>> FieldAnalyzers =
            new HashSet<IAnalyzer<ConstantMutation, FieldDefinition>>
            {
                new BooleanConstantAnalyzer(),
                new NumberConstantAnalyzer(),
                new StringConstantAnalyzer()
            };

        /// <summary>
        ///     Analyzers that search for possible opcode mutations.
        /// </summary>
        public HashSet<IAnalyzer<OpCodeMutation, Instruction>> OpCodeMethodAnalyzers =
            new HashSet<IAnalyzer<OpCodeMutation, Instruction>>
            {
                new ArithmeticAnalyzer(),
                new ComparisonAnalyzer(),
                new BitwiseAnalyzer()
            };

        /// <summary>
        ///     Analyzers that search for possible variable mutations.
        /// </summary>
        public HashSet<IAnalyzer<VariableMutation, MethodDefinition>> VariableMutationAnalyzers =
            new HashSet<IAnalyzer<VariableMutation, MethodDefinition>>
            {
                new VariableAnalyzer()
            };

        public AssemblyMutator(Stream stream)
        {
            Open(stream);
            Types = LoadTypes();
        }

        public AssemblyMutator(string assemblyPath) : this(new MemoryStream(File.ReadAllBytes(assemblyPath)))
        {
            Module = ModuleDefinition.ReadModule(assemblyPath, new ReaderParameters {InMemory = true});
            Types = LoadTypes();
        }

        /// <summary>
        ///     Underlying Mono.Cecil ModuleDefinition.
        /// </summary>
        public ModuleDefinition Module { get; private set; }

        /// <summary>
        ///     The types in the assembly.
        /// </summary>
        public List<FaultifyTypeDefinition> Types { get; }

        public void Dispose()
        {
            Module?.Dispose();
        }

        private List<FaultifyTypeDefinition> LoadTypes()
        {
            return Module.Types
                .Where(type => !type.FullName.StartsWith("<"))
                .Select(type => new FaultifyTypeDefinition(type, OpCodeMethodAnalyzers, FieldAnalyzers,
                    VariableMutationAnalyzers, ArrayMutationAnalyzers))
                .ToList();
        }

        public void Open(Stream stream)
        {
            Module = ModuleDefinition.ReadModule(stream);
        }

        /// <summary>
        ///     Flush the assembly changes to the given file.
        /// </summary>
        /// <param name="stream"></param>
        public void Flush(Stream stream)
        {
            Module.Write(stream);
        }

        public void Flush(string path)
        {
            Module.Write(path);
        }

        public void Flush()
        {
            Module.Write(Module.FileName);
        }
    }
}