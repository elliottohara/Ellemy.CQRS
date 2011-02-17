using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ellemy.CQRS.Config;
using Microsoft.CSharp;

namespace Ellemy.CQRS.Serializers.GoogleProtocolBuffersSerializer
{
    public class Serializer : ISerializer 
    {
        public object Deserialize(string input, Type desiredType)
        {
            var stream = new StreamReader(input);
            return ProtoBuf.Serializer.NonGeneric.Deserialize(desiredType, stream.BaseStream);
        }

        public object DeserializeObject(string input)
        {
            var stream = new StreamReader(input);
            return ProtoBuf.Serializer.NonGeneric.Deserialize(typeof(object),stream.BaseStream);
        }
        public string Serialize(object input)
        {
            string data;
            using (var writer = new MemoryStream())
            {
                ProtoBuf.Serializer.NonGeneric.Serialize(writer, input);
                writer.Position = 0;
                using (var reader = new StreamReader(writer))
                {
                    data = reader.ReadToEnd();
                }
            }
            return data;
        }
    }
    public class ProtoGenerator
    {
        
        
        public object GenerateProtoFor<T>(T thing)
        {
            var compileUnit = new CodeCompileUnit();
            var nameSpace = new CodeNamespace("Ellemy.CQRS.Serializers.GoogleProtocolBuffers.Contracts");
            nameSpace.Imports.Add(new CodeNamespaceImport("ProtoBuf"));
            var @class = new CodeTypeDeclaration(thing.GetType().Name)
                             {
                                 IsClass = true,
                                 Attributes = MemberAttributes.Public
                             };

            nameSpace.Types.Add(@class);
            var memberNumber = 1;
            foreach (var propertyInfo in thing.GetType().GetProperties())
            {
                var @public = new CodeMemberProperty();
                var protoBuffAttribute = new CodeAttributeDeclaration("ProtoMember");
                var attributeArgument = new CodeAttributeArgument(new CodePrimitiveExpression(memberNumber++));
                protoBuffAttribute.Arguments.Add(attributeArgument);
                @public.Attributes = MemberAttributes.Public;
                @public.Name = propertyInfo.Name;
                @public.CustomAttributes.Add(protoBuffAttribute);
                @class.Members.Add(@public);

            }
            
            compileUnit.Namespaces.Add(nameSpace);
            compileUnit.ReferencedAssemblies.Add("protobuf-net.dll");
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var writer = new IndentedTextWriter(new StreamWriter(@"C:\Temp\Test.cs", false));
            provider.GenerateCodeFromNamespace(nameSpace,writer,new CodeGeneratorOptions());
            return nameSpace;
            
        }
    }
}