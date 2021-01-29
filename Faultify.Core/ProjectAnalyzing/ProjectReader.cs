using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Buildalyzer;
using Buildalyzer.Environment;

namespace Faultify.Core.ProjectAnalyzing
{
    public class ProjectReader : IProjectReader
    {
        public Task<IProjectInfo> ReadProjectAsync(string path, IProgress<string> progress)
        {
            return Task.Run<IProjectInfo>(() =>
            {
                var analyzerManager = new AnalyzerManager();
                var projectAnalyzer = analyzerManager.GetProject(path);
                progress.Report($"Building {Path.GetFileName(path)}");
                var analyzerResult =
                    projectAnalyzer.Build(new EnvironmentOptions {DesignTime = false, Restore = true}).First();

                return new ProjectInfo(analyzerResult);
            });
        }

        public Task<IProjectInfo> LoadProjectAsync(string path, IProgress<string> progress)
        {
            return Task.Run<IProjectInfo>(() =>
            {
                var analyzerManager = new AnalyzerManager();
                var projectAnalyzer = analyzerManager.GetProject(path);
                progress.Report($"Building {Path.GetFileName(path)}");
                var analyzerResult =
                    projectAnalyzer.Build(new EnvironmentOptions { DesignTime = false, Restore = false}).First();

                return new ProjectInfo(analyzerResult);
            });
        }
    }
}