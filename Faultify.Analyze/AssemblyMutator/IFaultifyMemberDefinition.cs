using System.Reflection.Metadata;

namespace Faultify.Analyze.AssemblyMutator
{
    public interface IFaultifyMemberDefinition
    {
        /// <summary>
        ///     Full assembly name of this member.
        /// </summary>
        public string AssemblyQualifiedName { get; }

        /// <summary>
        ///     Assembly name of this member.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     The entity handle to this member.
        /// </summary>
        EntityHandle Handle { get; }
    }
}