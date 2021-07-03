using System.Collections.Generic;
using Faultify.Analyze.Mutation;

namespace Faultify.Analyze.MutationGroups
{
    /// <summary>
    ///     Groups a list of mutations along with some metadata for reporting purposes
    ///     This interface is required for the covariance <code>out</code> parameter
    ///     which cannot be added directly to a class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMutationGroup<out T> : IReportable, IEnumerable<T> where T : IMutation
    {
        public IEnumerable<T> Mutations { get; }
    }
}
