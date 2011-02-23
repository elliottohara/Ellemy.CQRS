using System;
using System.IO;
using System.Text;

namespace Ellemy.CQRS.Serializers.GoogleProtocolBuffersSerializer
{
    public class Serializer : ISerializer 
    {
        private readonly ProtocolBufferDataContractGenerator _protocolBufferDataContractGenerator;

        public Serializer()
        {
            _protocolBufferDataContractGenerator = new ProtocolBufferDataContractGenerator();
        }
        public object Deserialize(string input, Type desiredType)
        {
            var bytes = GetBytes(input);
            var @event = Activator.CreateInstance(desiredType);

            using (var stream = new MemoryStream(bytes))
            {
                var thisSucksINeedToFixIt = Activator.CreateInstance(desiredType);
                var protobufferType = _protocolBufferDataContractGenerator.GenerateProtoFor(thisSucksINeedToFixIt).GetType();
                var protobuffer = ProtoBuf.Serializer.NonGeneric.Deserialize(protobufferType, stream);
                
                foreach (var fieldInfo in protobuffer.GetType().GetProperties())
                {
                    var setter = desiredType.GetProperty(fieldInfo.Name);
                    var value = fieldInfo.GetValue(protobuffer,null);
                    setter.SetValue(@event, value, null);
                }
            }
            return @event;
        }

        private static byte[] GetBytes(string input)
        {
            //http://stackoverflow.com/questions/1230303/bitconverter-tostring-in-reverse

            var length = (input.Length + 1) / 3;
            var arr1 = new byte[length];
            for (int i = 0; i < length; i++)
            {
                var sixteen = input[3 * i];
                if (sixteen > '9') sixteen = (char)(sixteen - 'A' + 10);
                else sixteen -= '0';

                var ones = input[3 * i + 1];
                if (ones > '9') ones = (char)(ones - 'A' + 10);
                else ones -= '0';

                arr1[i] = (byte)(16 * sixteen + ones);
            }
            return arr1;
        }

        public object DeserializeObject(string input)
        {
            throw new NotSupportedException("nope, dis dont werk ");
        }
        public string Serialize(object input)
        {
            var t = _protocolBufferDataContractGenerator.GenerateProtoFor(input);
            foreach (var property in input.GetType().GetProperties())
            {
                var setterForT = t.GetType().GetProperty(property.Name);
                var value = property.GetValue(input, null);
                setterForT.SetValue(t, value,null);
            }
            string result;
            using (var writer = new MemoryStream())
            {
                ProtoBuf.Serializer.NonGeneric.Serialize(writer, t);
                writer.Position = 0;
                result = BitConverter.ToString(writer.ToArray());
            }
            return result;
        }
    }
}