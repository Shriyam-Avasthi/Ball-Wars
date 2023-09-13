using UnityEngine;
using Unity.Networking.Transport;

public class NetJoinRoom : NetMessage
{
    public ushort port;

    public NetJoinRoom( ushort _port)
    {
        Code = OpCode.JOIN_ROOM;
        port = _port;
    }

    public NetJoinRoom( DataStreamReader reader)
    {
        Code = OpCode.JOIN_ROOM;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte)Code);
        writer.WriteUShort(port);
    }

    public override void Deserialize( DataStreamReader reader)
    {
        port = reader.ReadUShort();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_JOIN_ROOM?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_JOIN_ROOM?.Invoke(this , Port  ,cnn);
    }
}
