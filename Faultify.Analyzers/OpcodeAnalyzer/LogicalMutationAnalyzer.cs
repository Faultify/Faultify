namespace Faultify.Analyzers.OpcodeAnalyzer
{
    /// <summary>
    ///     Analyzer that searches for possible logical mutations inside a method definition.
    ///     Mutations such as such as '&&' to '||'.
    ///     The logical operators are compiled to bitwise operations in IL-code.
    ///     Therefore the same mutation logic can be applied.
    /// </summary>
    public class LogicalMutationAnalyzer : BitwiseMutationAnalyzer
    {
        public override string Description =>
            "Analyzer that searches for possible logical mutations such as '&&' to '||'.";

        public override string Name => "Logical Analyzer";
    }
}