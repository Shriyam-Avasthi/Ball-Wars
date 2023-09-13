using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using System;

public enum OpCode
{
    KEEP_ALIVE = 0,
    BECOME_MASTER_CLIENT = 1,
    PLAYER_INFO = 2,
    PLAYER_INFO_LIST = 3,
    START_GAME = 4,
    GAME_LOADED = 5,
    LOAD_GAME = 6,
    NET_WELCOME = 7,
    INSTANTIATE = 8,
    HORIZONTAL_INPUT = 9,
    JUMP_INPUT = 10,
    SYNC_POSITION = 11,
    AIM_INPUT = 12,
    PROJECTILE_HIT = 13,
    RESPAWN = 14,
    UPDATE_SCORE = 15,
    APPLY_POWER_UP = 16,
    INSTANTIATE_POWER_UP = 17,
    CREATE_ROOM = 18,
    ROOM_INFO_LIST = 19,
    JOIN_ROOM = 20,
}

public class NetUtility : MonoBehaviour
{
    public static void OnData(DataStreamReader stream , ushort _port , NetworkConnection cnn , bool isServer)
    {
        NetMessage msg = null;
        var opCode = (OpCode)stream.ReadByte();
        
        switch(opCode)
        {
            case OpCode.KEEP_ALIVE : msg = new NetKeepAlive(stream); break;
            case OpCode.BECOME_MASTER_CLIENT :  msg = new NetBecomeMasterClient(stream); break;
            case OpCode.PLAYER_INFO : msg = new NetPlayerInfo(stream) ; break;
            case OpCode.PLAYER_INFO_LIST:  msg = new NetPlayerInfoList(stream); break;
            case OpCode.LOAD_GAME : msg = new NetLoadGame(stream); break;
            case OpCode.START_GAME : msg = new NetStartGame(stream); break;
            case OpCode.GAME_LOADED : msg = new NetGameLoaded(stream); break;
            case OpCode.NET_WELCOME : msg = new NetWelcome(stream); break;
            case OpCode.INSTANTIATE : msg = new NetInstantiate(stream); break;
            case OpCode.HORIZONTAL_INPUT : msg = new NetHorizontalInput(stream); break;
            case OpCode.JUMP_INPUT : msg = new NetJumpInput(stream); break;
            case OpCode.SYNC_POSITION : msg = new NetSyncPosition(stream); break;
            case OpCode.AIM_INPUT : msg = new NetAimInput(stream); break;
            case OpCode.PROJECTILE_HIT : msg = new NetProjectileHit(stream); break;
            case OpCode.RESPAWN : msg = new NetRespawn(stream); break;
            case OpCode.UPDATE_SCORE : msg = new NetUpdateScore(stream); break;
            case OpCode.APPLY_POWER_UP : msg = new NetApplyPowerUp(stream); break;
            case OpCode.INSTANTIATE_POWER_UP : msg = new NetInstantiatePowerUp(stream); break;
            case OpCode.CREATE_ROOM : msg = new NetCreateRoom(stream); break;
            case OpCode.ROOM_INFO_LIST : msg = new NetRoomInfoList(stream); break;
            case OpCode.JOIN_ROOM : msg = new NetJoinRoom(stream); break;
            default:
                Debug.LogError("Message received had no OpCode" + opCode );
                break;
        }
        if( isServer)
        {
            msg.Port = _port;
            msg.ReceivedOnServer(cnn);
        }
        else
            msg.ReceivedOnClient();
    }

   // Net Messages
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_PLAYER_INFO;
    public static Action<NetMessage> C_PLAYER_INFO_LIST;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_BECOME_MASTER_CLIENT;
    public static Action<NetMessage> C_GAME_LOADED;
    public static Action<NetMessage> C_LOAD_GAME;
    public static Action<NetMessage> C_NET_WELCOME;
    public static Action<NetMessage> C_INSTANTIATE;
    public static Action<NetMessage> C_HORIZONTAL_INPUT;
    public static Action<NetMessage> C_JUMP_INPUT;
    public static Action<NetMessage> C_SYNC_POSITION;
    public static Action<NetMessage> C_AIM_INPUT;
    public static Action<NetMessage> C_PROJECTILE_HIT;
    public static Action<NetMessage> C_RESPAWN;
    public static Action<NetMessage> C_UPDATE_SCORE;
    public static Action<NetMessage> C_APPLY_POWER_UP;
    public static Action<NetMessage> C_INSTANTIATE_POWER_UP;
    public static Action<NetMessage> C_CREATE_ROOM;
    public static Action<NetMessage> C_ROOM_INFO_LIST;
    public static Action<NetMessage> C_JOIN_ROOM;
    public static Action<NetMessage , ushort , NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage , ushort , NetworkConnection> S_PLAYER_INFO;
    public static Action<NetMessage , ushort , NetworkConnection> S_PLAYER_INFO_LIST;
    public static Action<NetMessage , ushort , NetworkConnection> S_START_GAME;
    public static Action<NetMessage , ushort , NetworkConnection> S_BECOME_MASTER_CLIENT;
    public static Action<NetMessage , ushort , NetworkConnection> S_GAME_LOADED;
    public static Action<NetMessage , ushort , NetworkConnection> S_LOAD_GAME;
    public static Action<NetMessage , ushort , NetworkConnection> S_NET_WELCOME;
    public static Action<NetMessage , ushort , NetworkConnection> S_INSTANTIATE;
    public static Action<NetMessage , ushort , NetworkConnection> S_HORIZONTAL_INPUT;
    public static Action<NetMessage , ushort , NetworkConnection> S_JUMP_INPUT;
    public static Action<NetMessage , ushort , NetworkConnection> S_SYNC_POSITION;
    public static Action<NetMessage , ushort , NetworkConnection> S_AIM_INPUT;
    public static Action<NetMessage , ushort , NetworkConnection> S_PROJECTILE_HIT;
    public static Action<NetMessage , ushort , NetworkConnection> S_RESPAWN;
    public static Action<NetMessage , ushort , NetworkConnection> S_UPDATE_SCORE;
    public static Action<NetMessage , ushort , NetworkConnection> S_APPLY_POWER_UP;
    public static Action<NetMessage , ushort , NetworkConnection> S_INSTANTIATE_POWER_UP;
    public static Action<NetMessage , ushort , NetworkConnection> S_CREATE_ROOM;
    public static Action<NetMessage , ushort , NetworkConnection> S_ROOM_INFO_LIST;
    public static Action<NetMessage , ushort , NetworkConnection> S_JOIN_ROOM;
}
