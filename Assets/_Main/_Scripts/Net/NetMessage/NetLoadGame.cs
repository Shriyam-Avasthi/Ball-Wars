using UnityEngine;
using Unity.Networking.Transport;

public class NetLoadGame : NetMessage
{
    public NetLoadGame()
    {
        Code = OpCode.LOAD_GAME;
    }

    public NetLoadGame( DataStreamReader reader)
    {
        Code = OpCode.LOAD_GAME;
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
        NetUtility.C_LOAD_GAME.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_LOAD_GAME?.Invoke(this , Port  ,cnn);
    }
}
