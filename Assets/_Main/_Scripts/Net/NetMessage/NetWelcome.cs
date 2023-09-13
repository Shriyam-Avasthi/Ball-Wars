using UnityEngine;
using Unity.Networking.Transport;

public class NetWelcome : NetMessage
{
    public int PlayerId{get; set;}
    public NetWelcome( int id)
    {
        Code = OpCode.NET_WELCOME;
        PlayerId = id;
    }

    public NetWelcome( DataStreamReader reader)
    {
        Code = OpCode.NET_WELCOME;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt( PlayerId );
    }

    public override void Deserialize( DataStreamReader reader)
    {
        PlayerId = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_NET_WELCOME?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_NET_WELCOME?.Invoke(this , Port  ,cnn);
    }
}
