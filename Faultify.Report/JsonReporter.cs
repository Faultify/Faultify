using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Faultify.Report
{
    public class JsonReporter : IReporter
    {
        public string FileExtension => ".json";

        public async Task<byte[]> CreateReportAsync(MutationProjectReportModel mutationRun)
        {
            using MemoryStream ms = new MemoryStream();
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            await JsonSerializer.SerializeAsync(ms, mutationRun, options);
            return ms.ToArray();
        }
    }
}
