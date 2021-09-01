// DO NOT EDIT: GENERATED BY VarIntBlocksTestGenerator.cs

using System;
using System.Collections;
using Mirage.Serialization;
using Mirage.Tests.Runtime.ClientServer;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Mirage.Tests.Runtime.Generated.VarIntBlocksTests
{
    
    public class VarIntBlocksBehaviour_uint_7 : NetworkBehaviour
    {
        [VarIntBlocks(7)]
        [SyncVar] public uint myValue;
    }
    public class VarIntBlocksTest_uint_7 : ClientServerSetup<VarIntBlocksBehaviour_uint_7>
    {
        [Test]
        [TestCase(10, 8)]
        [TestCase(100, 8)]
        [TestCase(1000, 16)]
        [TestCase(10000, 16)]

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
