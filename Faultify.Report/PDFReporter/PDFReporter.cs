using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RazorLight;
using WkHtmlToPdfDotNet;

namespace Faultify.Report.PDFReporter
{
    public class PdfReporter : IReporter
    {
        private static readonly BasicConverter Converter = new BasicConverter(new PdfTools());
        public string FileExtension => ".pdf";

        public async Task<byte[]> CreateReportAsync(MutationProjectReportModel mutationRun)
        {
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4Plus
                },
                Objects =
                {
                    new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = await Template(mutationRun),
                        WebSettings = {DefaultEncoding = "utf-8"},
                        HeaderSettings = {FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812}
                    }
                }
            };

            return Converter.Convert(doc);
        }

        private async Task<string> Template(MutationProjectReportModel model)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var resourceName = currentAssembly
                .GetManifestResourceNames()
                .Single(str => str.EndsWith("PDF.cshtml"));

            using var streamReader = new StreamReader(currentAssembly.GetManifestResourceStream(resourceName));
            var template = await streamReader.ReadToEndAsync();

            var engine = new RazorLightEngineBuilder()
                // required to have a default RazorLightProject type,
                // but not required to create a template from string.
                .UseEmbeddedResourcesProject(typeof(PdfReporter))
                .UseMemoryCachingProvider()
                .Build();

            var result = await engine.CompileRenderStringAsync("templateKey", template, model);
            return result;
        }
    }
}