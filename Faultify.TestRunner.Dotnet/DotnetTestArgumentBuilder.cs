using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Faultify.TestRunner.Dotnet
{
    [Obsolete("Moved into TestRunner.TestRun.TestHostRunners")]
    /// <summary>
    ///     Dotnet command argument builder.
    /// </summary>
    internal class DotnetTestArgumentBuilder
    {
        private readonly StringBuilder _arguments = new StringBuilder();

        public DotnetTestArgumentBuilder(string projectReference)
        {
            _arguments.Append($" test {projectReference}");
        }

        public DotnetTestArgumentBuilder WithoutLogo()
        {
            _arguments.Append(" --nologo");
            return this;
        }

        public DotnetTestArgumentBuilder WithCollector(string dataCollector)
        {
            _arguments.Append($" --collect {dataCollector}");
            return this;
        }

        public DotnetTestArgumentBuilder Silent()
        {
            _arguments.Append(" -v s");
            return this;
        }

        public DotnetTestArgumentBuilder WithTimeout(TimeSpan timeSpan)
        {
            _arguments.Append($" --blame-hang-timeout {timeSpan.TotalMilliseconds}ms");
            return this;
        }

        public DotnetTestArgumentBuilder WithTestAdapter(string testAdapterPath)
        {
            _arguments.Append($" --test-adapter-path \"{testAdapterPath}\"");
            return this;
        }

        public DotnetTestArgumentBuilder WithTests(IEnumerable<string> testNames)
        {
            _arguments.Append($" --filter \"{string.Join("|", testNames.Select(x => x))}\"");
            return this;
        }

        public DotnetTestArgumentBuilder DisableDump()
        {
            _arguments.Append("  --blame-hang-dump-type none");
            return this;
        }

        public string Build()
        {
            return _arguments.ToString();
        }
    }
}
