using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Faultify.Analyze.ConstantAnalyzer;
using Faultify.Analyze.OpcodeAnalyzer;
using Mono.Cecil;

namespace Faultify.Analyze.AssemblyMutator
{
    public class AssemblyMutator : IDisposable
    {
        public readonly HashSet<ArrayMutationAnalyzer> ArrayMutationAnalyzers = new HashSet<ArrayMutationAnalyzer>();

        public readonly HashSet<ConstantMutationAnalyzer> ConstantAnalyzers = new HashSet<ConstantMutationAnalyzer>
        {
            new StringConstantMutationAnalyzer(),
            new NumberConstantMutationAnalyzer()
        };

        public readonly HashSet<VariableMutationAnalyzer> VariableMutationAnalyzers =
            new HashSet<VariableMutationAnalyzer>
            {
                new VariableMutationAnalyzer()
            };

        /// <summary>
        ///     Analyzer that searches for possible constant mutations inside a method definition.
        /// </summary>
        public HashSet<ConstantMutationAnalyzer> FieldAnalyzers = new HashSet<ConstantMutationAnalyzer>
        {
            new BooleanConstantMutationAnalyzer(),
            new NumberConstantMutationAnalyzer(),
            new StringConstantMutationAnalyzer()
        };

        /// <summary>
        ///     Analyzers that searches for possible opcode mutations inside a method definition.
        /// </summary>
        public HashSet<OpCodeMutationAnalyzer> OpCodeMethodAnalyzers = new HashSet<OpCodeMutationAnalyzer>
        {
            new ArithmeticMutationAnalyzer(),
            new ComparisonMutationAnalyzer(),
            new BitwiseMutationAnalyzer()
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
                    ConstantAnalyzers, VariableMutationAnalyzers, ArrayMutationAnalyzers))
                .ToList();
        }

        public void Open(Stream stream)
        {
            Module = ModuleDefinition.ReadModule(stream);
        }

        /// <summary>
        ///     Flush the assembly changes to the given file.
        /// </summary>
        /// <param name="fullPath"></param>
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