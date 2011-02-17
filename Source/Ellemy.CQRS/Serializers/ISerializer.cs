using System;

namespace Ellemy.CQRS.Serializers
{
    public interface ISerializer
    {
        object Deserialize(string input, Type desiredType);
        object DeserializeObject(string input);
        string Serialize(object input);
    }
}