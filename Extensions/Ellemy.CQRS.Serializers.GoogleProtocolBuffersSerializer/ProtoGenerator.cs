using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CSharp;

namespace Ellemy.CQRS.Serializers.GoogleProtocolBuffersSerializer
{
    public class ProtoGenerator
    {
        private static readonly Dictionary<string,Type > Cache = new Dictionary<string, Type>();
        
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
            var serializable = new CodeAttributeDeclaration("Serializable");
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
            Cache[thing.GetType().FullName] = results.CompiledAssembly.GetType(typeName);

            return
                results.CompiledAssembly.CreateInstance("Ellemy.CQRS.Serializers.GoogleProtocolBuffers.Contracts." +
                                                        thing.GetType().Name);


        }
    }
}