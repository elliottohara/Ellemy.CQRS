using System;
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
            pg.GenerateProtoFor(testThing);
        }
    }
    [ProtoContract]
    public class TestThing
    {
        [ProtoMember(1)]
        public int Int { get; set; }
        [ProtoMember(2)]
        public string String { get; set; }
        [ProtoMember(3)]
        public Guid Guid { get; set; }
        [ProtoMember(4)]
        public Enum1 Enum1 { get; set; }
    }
    public enum Enum1
    {
        Val1,Val2,Val3
    }
}
