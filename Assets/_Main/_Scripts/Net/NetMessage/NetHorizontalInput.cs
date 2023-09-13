using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetHorizontalInput : NetMessage
{
    public int playerID;
    public float horizontalInput;
    public NetHorizontalInput( int id , float h_inp )
    {   
        Code = OpCode.HORIZONTAL_INPUT;
        playerID = id;
        horizontalInput = h_inp;
    }

    public NetHorizontalInput( DataStreamReader reader)
    {
        Code = OpCode.HORIZONTAL_INPUT;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt(playerID);
        writer.WriteFloat(horizontalInput);
    }

    public override void Deserialize( DataStreamReader reader)
    {
        playerID = reader.ReadInt();
        horizontalInput = reader.ReadFloat();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_HORIZONTAL_INPUT?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_HORIZONTAL_INPUT?.Invoke(this , Port  ,cnn);
    }
}
