using System;
using System.Linq;
using Ellemy.CQRS.Serializers.GoogleProtocolBuffersSerializer;
using NUnit.Framework;
using ProtoBuf;
using Serializer = Ellemy.CQRS.Serializers.GoogleProtocolBuffersSerializer.Serializer;

namespace GoogleProtocolBufferTests
{
    [TestFixture]
    public class when_dynamically_generating_proto_files
    {
        [Test]
        public void try_it()
        {
            var serializer = new Serializer();
            var testThing = new TestThing {Int = 1, Enum1 = Enum1.Val1, Guid = Guid.NewGuid(), String = "Test"};
            for(var times = 1; times<100;times++)
            {
                var startedAt = DateTime.Now;
                serializer.Serialize(testThing);
                Console.WriteLine("Took {0}", DateTime.Now.Subtract(startedAt).TotalMilliseconds);
            }
            Console.WriteLine(serializer.Serialize(testThing));

        }
        [Test]
        [Ignore("This is a todo, I just don't wanna forget where I'm at")]
        public void serialize_then_deserialize()
        {
            var serializer = new Serializer();
            var testThing = new TestThing {Enum1 = Enum1.Val3, Guid = Guid.NewGuid(), Int = 5, String = "Some STring"};
            var output = serializer.Serialize(testThing);
            var result = serializer.DeserializeObject(output);
            Console.WriteLine(result);
        }
    }
    public class TestThing
    {
        public Int32 Int { get; set; }
        public string String { get; set; }
        public Guid Guid { get; set; }
        public Enum1 Enum1 { get; set; }
    }
    public enum Enum1
    {
        Val1,Val2,Val3
    }
}
