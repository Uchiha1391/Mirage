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
    
    public class VarIntBlocksBehaviour_long_8 : NetworkBehaviour
    {
        [VarIntBlocks(8)]
        [SyncVar] public long myValue;
    }
    public class VarIntBlocksTest_long_8 : ClientServerSetup<VarIntBlocksBehaviour_long_8>
    {
        [Test]
        [TestCase(10, 9)]
        [TestCase(100, 9)]
        [TestCase(1000, 18)]
        [TestCase(10000, 18)]

        public void SyncVarIsBitPacked(long value, int expectedBitCount)
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
