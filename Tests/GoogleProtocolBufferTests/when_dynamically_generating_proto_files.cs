using System;
using System.Linq;
using System.Text;
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
        public void try_it_and_compare_to_EllemyJson()
        {
            var serializer = new Serializer();
            var jsonSerializer = new Ellemy.CQRS.Serializers.EllemyJsonSerializer();
            var random = new Random();
            for(var times = 1; times<100;times++)
            {
                var testThing = new TestThing { Int = random.Next(), Enum1 = Enum1.Val1, Guid = Guid.NewGuid(), String = Guid.NewGuid().ToString() };
            
                var startedAt = DateTime.Now;
                var result = serializer.Serialize(testThing);
                Console.WriteLine("Took {0} milliseconds to serialize with protocol buffer", DateTime.Now.Subtract(startedAt).TotalMilliseconds);
                Console.WriteLine(result);
                Console.WriteLine("Size {0}", ASCIIEncoding.ASCII.GetBytes(result).Length);
                var startedJsonAt = DateTime.Now;
                var jsonResult = jsonSerializer.Serialize(testThing);
                Console.WriteLine("took {0} milliseconds to use EllemyJsonSerializer",DateTime.Now.Subtract(startedJsonAt).TotalMilliseconds);
                Console.WriteLine(jsonResult);
                Console.WriteLine("Size {0}", ASCIIEncoding.ASCII.GetBytes(jsonResult).Length);
                
            }
            

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
