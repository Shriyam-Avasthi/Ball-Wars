using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetInstantiate : NetMessage
{
    public PlayerInfo playerInfo;
    public NetInstantiate( PlayerInfo info )
    {   
        Code = OpCode.INSTANTIATE;
        playerInfo = info;
    }

    public NetInstantiate( DataStreamReader reader)
    {
        Code = OpCode.INSTANTIATE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt(playerInfo.PlayerID);
        writer.WriteFixedString128( (FixedString128Bytes) playerInfo.nickName);
    }

    public override void Deserialize( DataStreamReader reader)
    {
        int playerID = reader.ReadInt();
        string nickName  = reader.ReadFixedString128().ToString();
        playerInfo = new PlayerInfo( nickName , playerID);
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_INSTANTIATE?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_INSTANTIATE?.Invoke(this , Port  ,cnn);
    }
}
