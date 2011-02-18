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
            Console.WriteLine(serializer.Serialize(testThing));
        }
        [Test]
        public void make_pronto()
        {
            var testThing = new TestThing { Int = 1, Enum1 = Enum1.Val1, Guid = Guid.NewGuid(), String = "Test" };
            var pg = new ProtoGenerator();
            var t = pg.GenerateProtoFor(testThing);
            foreach (var property in testThing.GetType().GetProperties())
            {
                var setterForT = t.GetType().GetField(property.Name);
                var value = property.GetValue(testThing, null);
                setterForT.SetValue(t,value);
            }
            var serializer = new Serializer();
            Console.WriteLine(serializer.Serialize(t));
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
