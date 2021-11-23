using System;
using System.Collections.Generic;
using System.Linq;

namespace Faultify.Report
{
    public class MutationProjectReportModel
    {
        public List<TestProjectReportModel> TestProjects { get; set; } = new();
        public string Name { get; set; }

        public TimeSpan TestDuration => new(TestProjects.Sum(x => x.TestSessionDuration.Ticks));

        public int TotalMutationsSurvived => TestProjects.Sum(x => x.MutationsSurvived);
        public int TotalMutationsKilled => TestProjects.Sum(x => x.MutationsKilled);
        public int TotalMutationsNoCoverage => TestProjects.Sum(x => x.MutationsNoCoverage);
        public int TotalMutationsTimedOut => TestProjects.Sum(x => x.MutationsTimedOut);
        public int TotalMutations => TestProjects.Sum(x => x.TotalMutations);

        public float ScorePercentage => TestProjects.Average(x => x.ScorePercentage);
    }
}