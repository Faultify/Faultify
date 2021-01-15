using System.Reflection.Metadata;

namespace Faultify.Core.ProjectAnalyzing
{
    public interface ICodeDecompiler
    {
        string Decompile(EntityHandle entityHandle);
    }
}