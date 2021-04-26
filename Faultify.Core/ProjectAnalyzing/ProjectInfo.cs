using System.Collections.Generic;
using System.IO;
using Buildalyzer;

namespace Faultify.Core.ProjectAnalyzing
{
    public class ProjectInfo : IProjectInfo
    {
        private readonly IAnalyzerResult _analyzerResult;


        private string _assemblyPath;

        public ProjectInfo(IAnalyzerResult analyzerResult)
        {
            _analyzerResult = analyzerResult;
        }

        public string ProjectFilePath => _analyzerResult.ProjectFilePath;

        public IEnumerable<string> ProjectReferences => _analyzerResult.ProjectReferences;
        public string AssemblyPath => _assemblyPath ??= Path.Combine(TargetDirectory, TargetFileName);
        public string ProjectName => GetProperty("ProjectName");

        public string TargetDirectory => GetProperty("TargetDir");

        public string TargetFileName => GetProperty("TargetFileName");

        private string GetProperty(string name)
        {
            return _analyzerResult.GetProperty(name);
        }
    }
}
