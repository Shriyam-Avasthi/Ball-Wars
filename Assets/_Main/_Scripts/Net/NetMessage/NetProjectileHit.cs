using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class NetProjectileHit : NetMessage
{
    public int hitterID;
    public int playerID;
    public int damage;
    public Vector3 hitForce;
    public NetProjectileHit( int _hitterID, int id  , int _damage,  Vector3 force )
    {   
        Code = OpCode.PROJECTILE_HIT;
        hitterID = _hitterID;
        playerID = id;
        damage = _damage;
        hitForce = force;
    }

    public NetProjectileHit( DataStreamReader reader)
    {
        Code = OpCode.PROJECTILE_HIT;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte( (byte) Code);
        writer.WriteInt(hitterID);
        writer.WriteInt(playerID);
        writer.WriteInt( damage );
        writer.WriteFloat( hitForce.x );
        writer.WriteFloat( hitForce.y );
        writer.WriteFloat( hitForce.z );
    }

    public override void Deserialize( DataStreamReader reader)
    {
        hitterID = reader.ReadInt();
        playerID = reader.ReadInt();
        damage = reader.ReadInt();
        hitForce = new Vector3( reader.ReadFloat() , reader.ReadFloat() , reader.ReadFloat() ) ;
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_PROJECTILE_HIT?.Invoke(this);
    }
    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_PROJECTILE_HIT?.Invoke(this , Port  ,cnn);
    }
}
