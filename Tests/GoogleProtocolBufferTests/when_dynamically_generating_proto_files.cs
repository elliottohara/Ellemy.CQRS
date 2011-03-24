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
                var testThing = new TestThing { Int = random.Next(), Guid = Guid.NewGuid(), String = Guid.NewGuid().ToString() };
            
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
        public void serialize_then_deserialize()
        {
            var serializer = new Serializer();
            var jsonSerializer = new Ellemy.CQRS.Serializers.EllemyJsonSerializer();
            for (var i = 0; i < 35; i++)
            {
                Console.WriteLine("**************************");
                var testThing = new TestThing {Guid = Guid.NewGuid(), Int = i, String = "Some STring" + i };
                var output = serializer.Serialize(testThing);
                var json = jsonSerializer.Serialize(testThing);
                var startedAt = DateTime.Now;
                var result = (TestThing) serializer.Deserialize(output, typeof (TestThing));
                Console.WriteLine("took {0} milliseconds",DateTime.Now.Subtract(startedAt).TotalMilliseconds);
                
                var startedJsonAt = DateTime.Now;
                jsonSerializer.Deserialize(json, typeof (TestThing));
                Console.WriteLine("JSON took {0} milliseconds",DateTime.Now.Subtract(startedJsonAt).TotalMilliseconds);
                Console.WriteLine(result.Guid);
                Console.WriteLine(result.Int);
                Console.WriteLine(result.String);
            }
        }
    }
    public class TestThing
    {
        public int Int { get; set; }
        public string String { get; set; }
        public Guid Guid { get; set; }
        //public Enum1 Enum1 { get; set; }
    }
    public enum Enum1
    {
        Val1,Val2,Val3
    }
}
