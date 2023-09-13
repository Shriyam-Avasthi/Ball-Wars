using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetUpdateScore : NetMessage
{
    public int playerID;
    public int score;
    public NetUpdateScore( int id, int _score )
    {   
        Debug.Log("Created");
        Code = OpCode.UPDATE_SCORE;
        playerID = id;
        score = _score;
    }

    public NetUpdateScore( DataStreamReader reader)
    {
        Code = OpCode.UPDATE_SCORE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt(playerID);
        writer.WriteInt( score );
    }

    public override void Deserialize( DataStreamReader reader)
    {
        playerID = reader.ReadInt();
        score = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_UPDATE_SCORE?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_UPDATE_SCORE?.Invoke(this , Port  ,cnn);
    }
}
