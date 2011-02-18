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
        private ProtoGenerator _protoGenerator;

        public Serializer()
        {
            _protoGenerator = new ProtoGenerator();
        }
        public object Deserialize(string input, Type desiredType)
        {
            var stream = new StreamReader(input);
           return ProtoBuf.Serializer.NonGeneric.Deserialize(desiredType, stream.BaseStream);
        }

        public object DeserializeObject(string input)
        {
            var stream = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(input));
            return ProtoBuf.Serializer.NonGeneric.Deserialize(typeof(object),stream);
        }
        public string Serialize(object input)
        {
            var t = _protoGenerator.GenerateProtoFor(input);
            foreach (var property in input.GetType().GetProperties())
            {
                var setterForT = t.GetType().GetField(property.Name);
                var value = property.GetValue(input, null);
                setterForT.SetValue(t, value);
            }
            string data;
            using (var writer = new MemoryStream())
            {
                ProtoBuf.Serializer.NonGeneric.Serialize(writer, t);
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
        public static Dictionary<string,Type > Cache = new Dictionary<string, Type>();
        
        //TODOL this performs like crap... lets singleton and cache this puppy
        public object GenerateProtoFor<T>(T thing)
        {
            if (Cache.ContainsKey(thing.GetType().FullName))
                return Activator.CreateInstance(Cache[thing.GetType().FullName]);

            var nameSpace = new CodeNamespace("Ellemy.CQRS.Serializers.GoogleProtocolBuffers.Contracts");
            nameSpace.Imports.Add(new CodeNamespaceImport("ProtoBuf"));
            nameSpace.Imports.Add(new CodeNamespaceImport(thing.GetType().Namespace));
            var @class = new CodeTypeDeclaration(thing.GetType().Name)
                             {
                                 IsClass = true,
                                 Attributes = MemberAttributes.Public
                             };
            var protoContractAttribute = new CodeAttributeDeclaration("ProtoContract");
            @class.CustomAttributes.Add(protoContractAttribute);
            nameSpace.Types.Add(@class);
            var memberNumber = 1;
            foreach (var propertyInfo in thing.GetType().GetProperties())
            {
                var @public = new CodeMemberField();
                var protoBuffAttribute = new CodeAttributeDeclaration("ProtoMember");
                var attributeArgument = new CodeAttributeArgument(new CodePrimitiveExpression(memberNumber++));
                protoBuffAttribute.Arguments.Add(attributeArgument);
                @public.Type = new CodeTypeReference(propertyInfo.PropertyType.FullName);
                @public.Attributes = MemberAttributes.Public;
                @public.Name = propertyInfo.Name;
                @public.CustomAttributes.Add(protoBuffAttribute);
                
                @class.Members.Add(@public);

            }
            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(nameSpace);
            compileUnit.ReferencedAssemblies.Add("protobuf-net.dll");
            var thingAssembly = thing.GetType().Assembly;
            var assemblyToAdd = thingAssembly.GetName().Name + ".dll";
            compileUnit.ReferencedAssemblies.Add(assemblyToAdd);
            var parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
           
            var provider = new CSharpCodeProvider();
            var results = provider.CompileAssemblyFromDom(parameters,compileUnit);
            if(results.Errors.Count != 0)
            {
                throw new InvalidOperationException(results.Errors[0].ErrorText);
            }
            var typeName = "Ellemy.CQRS.Serializers.GoogleProtocolBuffers.Contracts." +
                           thing.GetType().Name;
            Cache[typeName] = results.CompiledAssembly.GetType(typeName);

            return
                results.CompiledAssembly.CreateInstance("Ellemy.CQRS.Serializers.GoogleProtocolBuffers.Contracts." +
                                                        thing.GetType().Name);


        }
    }
}