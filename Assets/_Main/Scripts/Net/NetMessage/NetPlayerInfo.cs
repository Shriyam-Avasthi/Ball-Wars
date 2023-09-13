using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetPlayerInfo : NetMessage
{
    public PlayerInfo PlayerInfo{set; get;}
    public NetPlayerInfo( PlayerInfo info)
    {
        Code = OpCode.PLAYER_INFO;
        PlayerInfo = info;
    }

    public NetPlayerInfo( DataStreamReader reader)
    {
        Code = OpCode.PLAYER_INFO;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteFixedString128( (FixedString128Bytes)PlayerInfo.nickName );
    }

    public override void Deserialize( DataStreamReader reader)
    {
        string nickName = reader.ReadFixedString128().ToString();
        Debug.Log("DESEREALIXING");
        PlayerInfo = new PlayerInfo(nickName , 0);
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_PLAYER_INFO.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_PLAYER_INFO?.Invoke(this , Port  ,cnn);
    }
}
