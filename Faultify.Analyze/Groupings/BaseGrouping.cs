﻿using System.Collections;
using System.Collections.Generic;
using Faultify.Analyze.Mutation;

namespace Faultify.Analyze.Groupings
{
    /// <summary>
    ///     Base implementation for a mutation group.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseGrouping<T> : IMutationGrouping<T> where T : IMutation
    {
        public IEnumerator<T> GetEnumerator()
        {
            return Mutations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> Mutations { get; set; }
        public string Key { get; set; }
        public string AnalyzerDescription { get; set; }
        public string AnalyzerName { get; set; }

        public string Description => AnalyzerDescription;

        public string Name => AnalyzerName;

        public override string ToString()
        {
            return $"Analyzed by {AnalyzerName} ({AnalyzerDescription}), Key: {Key}";
        }
    }
}