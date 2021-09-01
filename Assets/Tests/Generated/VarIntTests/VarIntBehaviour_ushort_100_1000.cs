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
    
    public class VarIntBehaviour_ushort_100_1000 : NetworkBehaviour
    {
        [VarInt(100, 1000, 10000)]
        [SyncVar] public ushort myValue;
    }
    public class VarIntTest_ushort_100_1000 : ClientServerSetup<VarIntBehaviour_ushort_100_1000>
    {
        [Test]
        [TestCase(10, 8)]
        [TestCase(100, 8)]
        [TestCase(1000, 12)]
        [TestCase(10000, 16)]

        public void SyncVarIsBitPacked(ushort value, int expectedBitCount)
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
