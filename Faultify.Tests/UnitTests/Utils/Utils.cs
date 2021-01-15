using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Faultify.Tests.UnitTests.Utils
{
    public static class Utils
    {
        /// <summary>
        ///     Change a 'comparison' statement to the 'branching' complement.
        /// </summary>
        /// <param name="binary"></param>
        /// <param name="methodName"></param>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public static byte[] ChangeComparisonToBranchOperator(byte[] binary, string methodName, OpCode opcode)
        {
            var module = ModuleDefinition.ReadModule(new MemoryStream(binary, false));
            var method = module.Types.SelectMany(x => x.Methods).FirstOrDefault(x => x.Name == methodName);

            var cbg = new ConditionalBranchGenerator(opcode);
            cbg.ReplaceMethodInstructions(method);

            using (var ms = new MemoryStream())
            {
                module.Write(ms);
                return ms.ToArray();
            }
        }
    }
}