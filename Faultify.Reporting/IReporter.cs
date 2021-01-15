using System.Threading.Tasks;

namespace Faultify.Reporting
{
    public interface IReporter
    {
        string FileExtension { get; }
        Task<byte[]> CreateReport(MutationProjectReportModel mutationRun);
    }
}