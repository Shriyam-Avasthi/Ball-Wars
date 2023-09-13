using UnityEngine;
using Unity.Networking.Transport;

public class NetGameLoaded : NetMessage
{
    public NetGameLoaded()
    {
        Code = OpCode.GAME_LOADED;
    }

    public NetGameLoaded( DataStreamReader reader)
    {
        Code = OpCode.GAME_LOADED;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
    }

    public override void Deserialize( DataStreamReader reader)
    {

    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_GAME_LOADED?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_GAME_LOADED?.Invoke(this , Port  ,cnn);
    }
}
