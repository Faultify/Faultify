using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;

namespace Faultify.Core.ProjectAnalyzing
{
    public class CodeDecompiler : ICodeDecompiler
    {
        private readonly CSharpDecompiler _decompiler;

        public CodeDecompiler(string assemblyPath)
        {
            _decompiler = new CSharpDecompiler(assemblyPath, new DecompilerSettings());
        }

        public CodeDecompiler(string assemblyName, Stream stream)
        {
            // Necessary to create the decompiler.
            var settings = new DecompilerSettings();

            var file = new PEFile(assemblyName, stream);

            // Creates instance of CSharpDecompiler.
            _decompiler = new CSharpDecompiler(
                file,
                new UniversalAssemblyResolver(
                    assemblyName,
                    settings.ThrowOnAssemblyResolveErrors,
                    file.DetectTargetFrameworkId(),
                    null,
                    settings.LoadInMemory ? PEStreamOptions.PrefetchMetadata : PEStreamOptions.Default,
                    settings.ApplyWindowsRuntimeProjections
                        ? MetadataReaderOptions.ApplyWindowsRuntimeProjections
                        : MetadataReaderOptions.None
                ),
                settings
            );
        }

        /// Uses the CSharpDecompiler with the EntityHandle to return a string with a "Method", "Field" or "Type" (which is the whole class).
        public string Decompile(EntityHandle entityHandle)
        {
            return _decompiler.DecompileAsString(entityHandle);
        }
    }
}