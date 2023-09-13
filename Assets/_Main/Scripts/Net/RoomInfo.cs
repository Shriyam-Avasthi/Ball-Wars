using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo
{
    public ushort port;
    public string name;
    public int numOfPlayers;

    public RoomInfo( ushort _port , string _name)
    {
        port = _port;
        name = _name;
    }

    public RoomInfo( ushort _port , int _numOfPlayers , string _name)
    {
        port = _port;
        numOfPlayers = _numOfPlayers;
        name = _name;
    }
}
