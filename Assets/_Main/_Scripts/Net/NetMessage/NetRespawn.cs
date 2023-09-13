using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetRespawn : NetMessage
{
    public int playerID;
    public Vector3 respawnPosition;
    public NetRespawn( int id  ,  Vector3 pos )
    {   
        Code = OpCode.RESPAWN;
        playerID = id;
        respawnPosition = pos;
    }

    public NetRespawn( DataStreamReader reader)
    {
        Code = OpCode.RESPAWN;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt(playerID);
        writer.WriteFloat( respawnPosition.x );
        writer.WriteFloat( respawnPosition.y );
        writer.WriteFloat( respawnPosition.z );
    }

    public override void Deserialize( DataStreamReader reader)
    {
        playerID = reader.ReadInt();
        respawnPosition = new Vector3( reader.ReadFloat() , reader.ReadFloat() , reader.ReadFloat() ) ;
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_RESPAWN?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_RESPAWN?.Invoke(this , Port  ,cnn);
    }
}
