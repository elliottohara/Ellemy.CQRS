using System;
using System.IO;
using System.Linq;
using System.Text;
using Ellemy.CQRS.Config;

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
            var bytes = ASCIIEncoding.ASCII.GetBytes(input.Trim());
            var @event = Activator.CreateInstance(desiredType);
            using (var stream = new MemoryStream(bytes))
            {
                var thisSucksINeedToFixIt = Activator.CreateInstance(desiredType);
                var protobufferType = _protoGenerator.GenerateProtoFor(thisSucksINeedToFixIt).GetType();
                var protobuffer = ProtoBuf.Serializer.NonGeneric.Deserialize(protobufferType, stream);
                
                foreach (var fieldInfo in protobuffer.GetType().GetFields())
                {
                    var setter = desiredType.GetProperty(fieldInfo.Name);
                    var value = fieldInfo.GetValue(protobuffer);
                    setter.SetValue(@event, value, null);
                }
            }
            return @event;
        }

        public object DeserializeObject(string input)
        {
            throw new NotSupportedException("nope, dis dont werk ");
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
                using (var reader = new StreamReader(writer,Encoding.ASCII))
                {
                    data = reader.ReadToEnd();
                }
            }
            return data;
        }
    }
}