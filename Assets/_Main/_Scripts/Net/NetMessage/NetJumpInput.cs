using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetJumpInput : NetMessage
{
    public int playerID;
    public NetJumpInput( int id )
    {   
        Code = OpCode.JUMP_INPUT;
        playerID = id;
    }

    public NetJumpInput( DataStreamReader reader)
    {
        Code = OpCode.JUMP_INPUT;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt(playerID);
    }

    public override void Deserialize( DataStreamReader reader)
    {
        playerID = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_JUMP_INPUT?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_JUMP_INPUT?.Invoke(this , Port  ,cnn);
    }
}
