using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using ModuleDefinition = Mono.Cecil.ModuleDefinition;

namespace Faultify.Tests.UnitTests.Utils
{
    internal static class DecompileHandleHelper
    {
        public static EntityHandle DecompileMethod(ModuleDefinition module, string typeName, string methodName,
            params Type[] typeList)
        {
            // Searches for the class / type
            var typeDefinition = module.Types.FirstOrDefault(type => type.Name == typeName);

            // Searches for the method
            var methodDefinition = typeDefinition.Methods.FirstOrDefault(x =>
                x.Name == methodName &&
                typeList.SequenceEqual(x.Parameters.Select(para => Type.GetType(para.ParameterType.FullName))));

            // Creates EntityHandle of methodDefinition
            var handle = MetadataTokens.EntityHandle(methodDefinition.MetadataToken.ToInt32());

            // Returns EntityHandle of Method.
            return handle;
        }

        public static EntityHandle DecompileType(ModuleDefinition module, string typeName)
        {
            // Searches for the class / type
            var typeDefinition = module.Types.FirstOrDefault(type => type.Name == typeName);

            // Creates EntityHandle of typeDefinition
            var handle = MetadataTokens.EntityHandle(typeDefinition.MetadataToken.ToInt32());

            // Returns EntityHandle of Type.
            return handle;
        }

        public static EntityHandle DecompileField(ModuleDefinition module, string typeName, string fieldName)
        {
            // Searches for the class / type
            var typeDefinition = module.Types.FirstOrDefault(type => type.Name == typeName);

            // Searches for the field
            var fieldDefinition = typeDefinition.Fields.FirstOrDefault(field => field.Name == fieldName);

            // Creates EntityHandle of fieldDefinition
            var handle = MetadataTokens.EntityHandle(fieldDefinition.MetadataToken.ToInt32());

            // Returns EntityHandle of Field.
            return handle;
        }
    }
}