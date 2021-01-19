using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Faultify.Analyze.ConstantAnalyzer;
using Faultify.Analyze.OpcodeAnalyzer;
using TypeDefinition = Mono.Cecil.TypeDefinition;

namespace Faultify.Analyze.AssemblyMutator
{
    /// <summary>
    ///     Represents a raw type definition and provides access to its fields and methods..
    /// </summary>
    public class FaultifyTypeDefinition : IFaultifyMemberDefinition
    {
        private readonly HashSet<ConstantMutationAnalyzer> _constantAnalyzers;

        public FaultifyTypeDefinition(TypeDefinition typeDefinition,
            HashSet<OpCodeMutationAnalyzer> methodAnalyzers, HashSet<ConstantMutationAnalyzer> fieldAnalyzers,
            HashSet<ConstantMutationAnalyzer> constantAnalyzers,
            HashSet<VariableMutationAnalyzer> variableMutationAnalyzers,
            HashSet<ArrayMutationAnalyzer> arrayMutationAnalyzers
        )
        {
            _constantAnalyzers = constantAnalyzers;
            TypeDefinition = typeDefinition;

            Fields = TypeDefinition.Fields.Select(x => new FaultifyFieldDefinition(x, fieldAnalyzers)).ToList();
            Methods = TypeDefinition.Methods.Select(x =>
                    new FaultifyMethodDefinition(x, methodAnalyzers, variableMutationAnalyzers, arrayMutationAnalyzers))
                .ToList();
        }

        /// <summary>
        ///     The fields in this type.
        ///     For example: const, static, non-static fields.
        /// </summary>
        public List<FaultifyFieldDefinition> Fields { get; }

        /// <summary>
        ///     The methods in this type.
        /// </summary>
        public List<FaultifyMethodDefinition> Methods { get; }

        public TypeDefinition TypeDefinition { get; }

        public string Name => TypeDefinition.Name;
        public EntityHandle Handle => MetadataTokens.EntityHandle(TypeDefinition.MetadataToken.ToInt32());
        public string AssemblyQualifiedName => TypeDefinition.FullName;
    }
}