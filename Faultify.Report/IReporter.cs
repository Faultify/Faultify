using System.Threading.Tasks;

namespace Faultify.Report
{
    public interface IReporter
    {
        string FileExtension { get; }
        Task<byte[]> CreateReportAsync(MutationProjectReportModel mutationRun);
    }
}