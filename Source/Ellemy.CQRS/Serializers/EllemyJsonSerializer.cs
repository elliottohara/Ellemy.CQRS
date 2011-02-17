using System;
using System.Web.Script.Serialization;

namespace Ellemy.CQRS.Serializers
{
    public class EllemyJsonSerializer : ISerializer
    {
        private readonly JavaScriptSerializer _serializer;

        public EllemyJsonSerializer()
        {
            _serializer = new JavaScriptSerializer();
        }

        public object Deserialize(string input, Type desiredType)
        {
            return _serializer.Deserialize(input, desiredType);
        }
        public object DeserializeObject(string input)
        {
            return _serializer.DeserializeObject(input);
        }
        public string Serialize(object input)
        {
            return _serializer.Serialize(input);
        }
    }
}