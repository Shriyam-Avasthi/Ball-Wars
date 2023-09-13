using UnityEngine;
using Unity.Networking.Transport;
public class NetApplyPowerUp : NetMessage
{
    public PowerUpType Type{ set; get; }
    public int targetPlayerID;
    public int powerUpInstanceID;

    public NetApplyPowerUp(PowerUpType type , int id , int p_upInstanceID)
    {
        Code = OpCode.APPLY_POWER_UP;
        Type = type;
        targetPlayerID = id;
        powerUpInstanceID = p_upInstanceID;
    }

    public NetApplyPowerUp( DataStreamReader reader)
    {
        Code = OpCode.APPLY_POWER_UP;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteByte( (byte) Type);
        writer.WriteInt(targetPlayerID);
        writer.WriteInt(powerUpInstanceID);
    }

    public override void Deserialize( DataStreamReader reader)
    {
        Type = (PowerUpType)reader.ReadByte();
        targetPlayerID = reader.ReadInt();
        powerUpInstanceID = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_APPLY_POWER_UP?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_APPLY_POWER_UP?.Invoke(this , Port  ,cnn);
    }
}
