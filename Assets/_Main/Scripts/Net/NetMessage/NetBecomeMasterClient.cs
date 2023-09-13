using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Collections.Generic;

public class NetBecomeMasterClient : NetMessage
{
    public NetBecomeMasterClient()
    {
        Code = OpCode.BECOME_MASTER_CLIENT;
    }

    public NetBecomeMasterClient( DataStreamReader reader)
    {
        Code = OpCode.BECOME_MASTER_CLIENT;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
    }

    public override void Deserialize( DataStreamReader reader)
    {
        Debug.Log("Deserializing Become master client");
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_BECOME_MASTER_CLIENT.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_BECOME_MASTER_CLIENT?.Invoke(this , Port  ,cnn);
    }
}
