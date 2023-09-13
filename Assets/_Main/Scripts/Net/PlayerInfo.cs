using Unity.Networking.Transport;
using UnityEngine;


public class PlayerInfo
{
    public string nickName;
    public int PlayerID{ get; set;}
    public NetworkConnection connection ;
    public int score = 0;
    public bool isPoweredUp = false;

    public PlayerInfo(string _name , int id , int _score = 0)
    {
        nickName = _name;
        PlayerID = id;
        score = _score;
    }

    public PlayerInfo( string _name , int id , NetworkConnection _cnn , int _score = 0) 
    {
        nickName = _name;
        PlayerID = id;
        connection = _cnn;
        score = _score;
    }

}
