using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetAimInput : NetMessage
{
    public int playerID;
    public Vector2 aimInput;
    public NetAimInput( int id , Vector2 aim_inp )
    {   
        Code = OpCode.AIM_INPUT;
        playerID = id;
        aimInput = aim_inp;
    }

    public NetAimInput( DataStreamReader reader)
    {
        Code = OpCode.AIM_INPUT;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt(playerID);
        writer.WriteFloat(aimInput.x);
        writer.WriteFloat(aimInput.y);        
    }

    public override void Deserialize( DataStreamReader reader)
    {
        playerID = reader.ReadInt();
        float x = reader.ReadFloat();
        float y = reader.ReadFloat();
        aimInput = new Vector2( x , y );
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_AIM_INPUT?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_AIM_INPUT?.Invoke(this , Port  ,cnn);
    }
}
