﻿using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RazorLight;

namespace Faultify.Report.HTMLReporter
{
    public class HtmlReporter : IReporter
    {
        public string FileExtension { get; } = ".html";

        public async Task<byte[]> CreateReportAsync(MutationProjectReportModel mutationRun)
        {
            return Encoding.UTF8.GetBytes(await Template(mutationRun));
        }

        private async Task<string> Template(MutationProjectReportModel model)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var resourceName = currentAssembly
                .GetManifestResourceNames()
                .Single(str => str.EndsWith("HTML.cshtml"));

            using var streamReader = new StreamReader(currentAssembly.GetManifestResourceStream(resourceName));
            var template = await streamReader.ReadToEndAsync();

            var engine = new RazorLightEngineBuilder()
                // required to have a default RazorLightProject type,
                // but not required to create a template from string.
                .UseEmbeddedResourcesProject(typeof(HtmlReporter))
                .UseMemoryCachingProvider()
                .Build();

            var result = await engine.CompileRenderStringAsync("templateKey", template, model);
            return result;
        }
    }
}