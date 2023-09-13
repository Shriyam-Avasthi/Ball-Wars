using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Collections.Generic;

public class NetCreateRoom : NetMessage
{
    public string name;

    public NetCreateRoom(string _name)
    {
        Code = OpCode.CREATE_ROOM;
        name = _name;
    }

    public NetCreateRoom( DataStreamReader reader)
    {
        Code = OpCode.CREATE_ROOM;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteFixedString128( (FixedString128Bytes)name );
    }

    public override void Deserialize( DataStreamReader reader)
    {
        name = reader.ReadFixedString128().ToString();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_CREATE_ROOM?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_CREATE_ROOM?.Invoke(this , Port  ,cnn);
    }
}
