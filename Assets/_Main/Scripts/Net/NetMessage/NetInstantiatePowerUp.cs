using UnityEngine;
using Unity.Networking.Transport;
public class NetInstantiatePowerUp : NetMessage
{
    public Vector3 position;
    public int powerUpInfoID;
    public int powerUpInstanceID;

    public NetInstantiatePowerUp( Vector3 pos , int p_UpInfoID ,  int p_UpInstanceID)
    {
        Code = OpCode.INSTANTIATE_POWER_UP;
        position = pos;
        powerUpInfoID = p_UpInfoID;
        powerUpInstanceID = p_UpInstanceID;
    }

    public NetInstantiatePowerUp( DataStreamReader reader)
    {
        Code = OpCode.INSTANTIATE_POWER_UP;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteFloat(position.x);
        writer.WriteFloat(position.y);
        writer.WriteFloat(position.z);
        writer.WriteInt( powerUpInfoID ); 
        writer.WriteInt(powerUpInstanceID);
    }

    public override void Deserialize( DataStreamReader reader)
    {
        position = new Vector3( reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat() );
        powerUpInfoID = reader.ReadInt();
        powerUpInstanceID = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_INSTANTIATE_POWER_UP?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_INSTANTIATE_POWER_UP?.Invoke(this , Port  ,cnn);
    }
}
