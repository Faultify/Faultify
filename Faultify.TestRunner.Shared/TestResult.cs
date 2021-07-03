using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Faultify.TestRunner.Shared
{
    public class TestResult
    {
        public string Name { get; set; }
        public TestOutcome Outcome { get; set; }
    }
}
