using System.IO;
using System.Text;
using System.Threading.Tasks;
using RazorLight;

namespace Faultify.Reporting.HTMLReporter
{
    public class HtmlReporter : IReporter
    {
        private readonly string _template = File.ReadAllText(Path.Combine("Reporters", "HTMLReporter", "Page.cshtml"));
        public string FileExtension { get; } = "HTML";

        public async Task<byte[]> CreateReport(MutationProjectReportModel mutationRun)
        {
            return Encoding.UTF8.GetBytes(await Template(mutationRun));
        }

        private async Task<string> Template(MutationProjectReportModel model)
        {
            var engine = new RazorLightEngineBuilder()
                // required to have a default RazorLightProject type,
                // but not required to create a template from string.
                .UseEmbeddedResourcesProject(typeof(HtmlReporter))
                .UseMemoryCachingProvider()
                .Build();

            var result = await engine.CompileRenderStringAsync("templateKey", _template, model);
            return result;
        }
    }
}