// DO NOT EDIT: GENERATED BY BitCountTestGenerator.cs

using System;
using System.Collections;
using Mirage.Serialization;
using Mirage.Tests.Runtime.ClientServer;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Mirage.Tests.Runtime.Generated.BitCountAttributeTests
{
    
    public class BitCountBehaviour_ulong_64 : NetworkBehaviour
    {
        [BitCount(64)]
        [SyncVar] public ulong myValue;

        public event Action<ulong> onRpc;

        [ClientRpc]
        public void RpcSomeFunction([BitCount(64)] ulong myParam)
        {
            onRpc?.Invoke(myParam);
        }
    }
    public class BitCountTest_ulong_64 : ClientServerSetup<BitCountBehaviour_ulong_64>
    {
        const ulong value = 20;

        [Test]
        public void SyncVarIsBitPacked()
        {
            serverComponent.myValue = value;

            using (PooledNetworkWriter writer = NetworkWriterPool.GetWriter())
            {
                serverComponent.SerializeSyncVars(writer, true);

                Assert.That(writer.BitPosition, Is.EqualTo(64));

                using (PooledNetworkReader reader = NetworkReaderPool.GetReader(writer.ToArraySegment()))
                {
                    clientComponent.DeserializeSyncVars(reader, true);
                    Assert.That(reader.BitPosition, Is.EqualTo(64));

                    Assert.That(clientComponent.myValue, Is.EqualTo(value));
                }
            }
        }

        // [UnityTest]
        // [Ignore("Rpc not supported yet")]
        public IEnumerator RpcIsBitPacked()
        {

            int called = 0;
            clientComponent.onRpc += (v) => { called++; Assert.That(v, Is.EqualTo(value)); };

            client.MessageHandler.UnregisterHandler<RpcMessage>();
            int payloadSize = 0;
            client.MessageHandler.RegisterHandler<RpcMessage>((player, msg) =>
            {
                // store value in variable because assert will throw and be catch by message wrapper
                payloadSize = msg.payload.Count;
                clientObjectManager.OnRpcMessage(msg);
            });


            serverComponent.RpcSomeFunction(value);
            yield return null;
            Assert.That(called, Is.EqualTo(1));
            Assert.That(payloadSize, Is.EqualTo(8), $"64 bits is 8 bytes in payload");
        }
    }
}
