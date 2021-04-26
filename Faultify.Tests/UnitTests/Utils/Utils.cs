extern alias MC;
using System.IO;
using System.Linq;
using MC::Mono.Cecil;
using MC::Mono.Cecil.Cil;

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
            ModuleDefinition module = ModuleDefinition.ReadModule(new MemoryStream(binary, false));
            MethodDefinition method = module.Types.SelectMany(x => x.Methods).FirstOrDefault(x => x.Name == methodName);

            ConditionalBranchGenerator cbg = new ConditionalBranchGenerator(opcode);
            cbg.ReplaceMethodInstructions(method);

            using (MemoryStream ms = new MemoryStream())
            {
                module.Write(ms);
                return ms.ToArray();
            }
        }
    }
}
