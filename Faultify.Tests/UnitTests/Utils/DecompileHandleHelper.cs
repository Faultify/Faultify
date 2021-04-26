using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using FieldDefinition = Mono.Cecil.FieldDefinition;
using MethodDefinition = Mono.Cecil.MethodDefinition;
using ModuleDefinition = Mono.Cecil.ModuleDefinition;
using TypeDefinition = Mono.Cecil.TypeDefinition;

namespace Faultify.Tests.UnitTests.Utils
{
    internal static class DecompileHandleHelper
    {
        public static EntityHandle DecompileMethod(
            ModuleDefinition module,
            string typeName,
            string methodName,
            params Type[] typeList
        )
        {
            // Searches for the class / type
            TypeDefinition typeDefinition = module.Types.FirstOrDefault(type => type.Name == typeName);

            // Searches for the method
            MethodDefinition methodDefinition = typeDefinition.Methods.FirstOrDefault(x =>
                x.Name == methodName
                && typeList.SequenceEqual(x.Parameters.Select(para => Type.GetType(para.ParameterType.FullName))));

            // Creates EntityHandle of methodDefinition
            EntityHandle handle = MetadataTokens.EntityHandle(methodDefinition.MetadataToken.ToInt32());

            // Returns EntityHandle of Method.
            return handle;
        }

        public static EntityHandle DecompileType(ModuleDefinition module, string typeName)
        {
            // Searches for the class / type
            TypeDefinition typeDefinition = module.Types.FirstOrDefault(type => type.Name == typeName);

            // Creates EntityHandle of typeDefinition
            EntityHandle handle = MetadataTokens.EntityHandle(typeDefinition.MetadataToken.ToInt32());

            // Returns EntityHandle of Type.
            return handle;
        }

        public static EntityHandle DecompileField(ModuleDefinition module, string typeName, string fieldName)
        {
            // Searches for the class / type
            TypeDefinition typeDefinition = module.Types.FirstOrDefault(type => type.Name == typeName);

            // Searches for the field
            FieldDefinition fieldDefinition = typeDefinition.Fields.FirstOrDefault(field => field.Name == fieldName);

            // Creates EntityHandle of fieldDefinition
            EntityHandle handle = MetadataTokens.EntityHandle(fieldDefinition.MetadataToken.ToInt32());

            // Returns EntityHandle of Field.
            return handle;
        }
    }
}
