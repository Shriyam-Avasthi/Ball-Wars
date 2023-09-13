using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Collections.Generic;

public class NetPlayerInfoList : NetMessage
{
    public List<PlayerInfo> PlayerInfoList{set; get;}
    public int ListLength;
    public NetPlayerInfoList(List<PlayerInfo> infos)
    {
        PlayerInfoList = new List<PlayerInfo>();
        Code = OpCode.PLAYER_INFO_LIST;
        ListLength = infos.Count;
        PlayerInfoList = infos;
    }

    public NetPlayerInfoList( DataStreamReader reader)
    {
        PlayerInfoList = new List<PlayerInfo>();
        Code = OpCode.PLAYER_INFO_LIST;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt( ListLength);
        for( int i = 0 ; i < ListLength ; i++ )
        {
            writer.WriteFixedString128( (FixedString128Bytes)PlayerInfoList[i].nickName );
            writer.WriteInt( PlayerInfoList[i].PlayerID );
            writer.WriteInt( PlayerInfoList[i].score );
        }
    }

    public override void Deserialize( DataStreamReader reader)
    {
        ListLength = reader.ReadInt();
        for( int i = 0 ; i < ListLength ; i++)
        {
            string nickName = reader.ReadFixedString128().ToString() ;
            int playerID = reader.ReadInt();
            int score = reader.ReadInt();
            PlayerInfoList.Add( new PlayerInfo(nickName , playerID , score )  );
        }
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_PLAYER_INFO_LIST?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_PLAYER_INFO_LIST?.Invoke(this , Port  ,cnn);
    }
}
