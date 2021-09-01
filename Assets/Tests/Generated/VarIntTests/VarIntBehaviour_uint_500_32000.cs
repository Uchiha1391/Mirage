// DO NOT EDIT: GENERATED BY VarIntTestGenerator.cs

using System;
using System.Collections;
using Mirage.Serialization;
using Mirage.Tests.Runtime.ClientServer;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Mirage.Tests.Runtime.Generated.VarIntTests
{
    
    public class VarIntBehaviour_uint_500_32000 : NetworkBehaviour
    {
        [VarInt(500, 32000, 2000000)]
        [SyncVar] public uint myValue;
    }
    public class VarIntTest_uint_500_32000 : ClientServerSetup<VarIntBehaviour_uint_500_32000>
    {
        [Test]
        [TestCase(170, 10)]
        [TestCase(500, 10)]
        [TestCase(15000, 17)]
        [TestCase(50000, 23)]
        [TestCase(400000, 23)]

        public void SyncVarIsBitPacked(uint value, int expectedBitCount)
        {
            serverComponent.myValue = value;

            using (PooledNetworkWriter writer = NetworkWriterPool.GetWriter())
            {
                serverComponent.SerializeSyncVars(writer, true);

                Assert.That(writer.BitPosition, Is.EqualTo(expectedBitCount));

                using (PooledNetworkReader reader = NetworkReaderPool.GetReader(writer.ToArraySegment()))
                {
                    clientComponent.DeserializeSyncVars(reader, true);
                    Assert.That(reader.BitPosition, Is.EqualTo(expectedBitCount));

                    Assert.That(clientComponent.myValue, Is.EqualTo(value));
                }
            }
        }
    }
}
