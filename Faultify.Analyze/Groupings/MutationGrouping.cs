using System.Collections;
using System.Collections.Generic;
using Faultify.Analyze.Mutation;

namespace Faultify.Analyze.Groupings
{
    /// <summary>
    ///     Base implementation for a mutation group.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MutationGrouping<T> : IMutationGrouping<T> where T : IMutation
    {
        public IEnumerator<T> GetEnumerator()
        {
            return Mutations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Mutations.GetEnumerator();
        }

        public IEnumerable<T> Mutations { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Analyzed by {Name} ({Description})";
        }
    }

    /// <summary>
    /// Shorthand for MutationGrouping<VariableMutation>
    /// <seealso cref="MutationGrouping"/>
    /// </summary>
    public class VariableMutationGrouping : MutationGrouping<VariableMutation> { }

    /// <summary>
    /// Shorthand for MutationGrouping<ArrayMutation>
    /// <seealso cref="MutationGrouping"/>
    /// </summary>
    public class ArrayMutationGrouping : MutationGrouping<ArrayMutation> { }

    /// <summary>
    /// Shorthand for MutationGrouping<OpCodeMutation>
    /// <seealso cref="MutationGrouping"/>
    /// </summary>
    public class OpCodeGrouping : MutationGrouping<OpCodeMutation> { }

    /// <summary>
    /// Shorthand for MutationGrouping<ConstantMutation>
    /// <seealso cref="MutationGrouping"/>
    /// </summary>
    public class ConstGrouping : MutationGrouping<ConstantMutation> { }
}