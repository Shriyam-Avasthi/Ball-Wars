using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetSyncPosition : NetMessage
{
    public int playerID;
    public Vector3 position;
    public NetSyncPosition( int id , Vector3 pos )
    {   
        Code = OpCode.SYNC_POSITION;
        playerID = id;
        position = pos;
    }

    public NetSyncPosition( DataStreamReader reader)
    {
        Code = OpCode.SYNC_POSITION;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt(playerID);
        writer.WriteFloat(position.x);
        writer.WriteFloat(position.y);
        writer.WriteFloat(position.z);
    }

    public override void Deserialize( DataStreamReader reader)
    {
        playerID = reader.ReadInt();
        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        float z = reader.ReadFloat();
        position = new Vector3( x , y , z );
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_SYNC_POSITION?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_SYNC_POSITION?.Invoke(this , Port  ,cnn);
    }
}
