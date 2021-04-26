using System;
using System.Threading.Tasks;

namespace Faultify.Core.ProjectAnalyzing
{
    public interface IProjectReader
    {
        Task<IProjectInfo> ReadProjectAsync(string path, IProgress<string> progress);
    }
}
