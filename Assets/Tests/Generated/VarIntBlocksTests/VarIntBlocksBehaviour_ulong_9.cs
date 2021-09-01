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
    
    public class VarIntBlocksBehaviour_ulong_9 : NetworkBehaviour
    {
        [VarIntBlocks(9)]
        [SyncVar] public ulong myValue;
    }
    public class VarIntBlocksTest_ulong_9 : ClientServerSetup<VarIntBlocksBehaviour_ulong_9>
    {
        [Test]
        [TestCase(10, 10)]
        [TestCase(100, 10)]
        [TestCase(1000, 20)]
        [TestCase(10000, 20)]

        public void SyncVarIsBitPacked(ulong value, int expectedBitCount)
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
