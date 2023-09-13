using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using System.Collections.Generic;

public class NetRoomInfoList : NetMessage
{
    public List<RoomInfo> roomInfos = new List<RoomInfo>();
    public int listLength;

    public NetRoomInfoList( List<RoomInfo> _roomInfos)
    {
        Code = OpCode.ROOM_INFO_LIST;
        roomInfos = _roomInfos;
    }

    public NetRoomInfoList( DataStreamReader reader)
    {
        Code = OpCode.ROOM_INFO_LIST;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt( roomInfos.Count );
        for( int i = 0; i < roomInfos.Count; i++)
        {
            writer.WriteUShort( roomInfos[i].port );
            writer.WriteInt( roomInfos[i].numOfPlayers );
            writer.WriteFixedString128( (FixedString128Bytes)roomInfos[i].name );
        }
    }

    public override void Deserialize( DataStreamReader reader)
    {
        listLength = reader.ReadInt();
        for( int i = 0; i < listLength; i++)
        {
            ushort port = reader.ReadUShort();
            int numOfPlayers = reader.ReadInt();
            string name = reader.ReadFixedString128().ToString();
            roomInfos.Add( new RoomInfo(port , numOfPlayers, name));
        }
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_ROOM_INFO_LIST?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_ROOM_INFO_LIST?.Invoke(this , Port  ,cnn);
    }
}
