using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System;
using System.Collections.Generic;

public class Server : MonoBehaviour
{
    public ushort port = 8008;
    public string name;

    private NetworkDriver driver;
    private NativeList<NetworkConnection> connections;
    public List<PlayerInfo> players;
    private List<int> allottedPowerUpIDs;

    private bool isActive = false;
    private System.Random random;
    private const float keepAliveTickRate = 20.0f;
    private float lastKeepAlive;
    private int playersLoaded = 0 ;
    private float lastPowerUpSpawnTimeElapsed;
    private float thisPowerUpSpawnTime;
    private bool shouldSpawnPowerUps = false;

    [SerializeField] private float minPowerUpSpawnTime = 10;
    [SerializeField] private float maxPowerUpSpawnTime = 20;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        RegisterEvents();
    }

    public void Init()
    {
        isActive = true;
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = port;
        if( driver.Bind(endpoint) != 0)
        {
            Debug.Log("SERVER : Unable to bind to the port" + endpoint.Port);
            return;
        }
        else 
        {
            driver.Listen();
            // Debug.Log("Server started.");
        }
        connections = new NativeList<NetworkConnection>( 4 , Allocator.Persistent);
        players = new List<PlayerInfo>();
        allottedPowerUpIDs = new List<int>();
        random = new System.Random();
    }
    public void Shutdown()
    {
        if(isActive)
        {
            // Debug.Log("SERVER : Shutting down");
            driver.Dispose();
            connections.Dispose();
            isActive = false;
        }
    }

    public void OnDestroy()
    {
        Shutdown();
        UnregisterEvents();
    }

    public void KeepAlive()
    {
        if( lastKeepAlive >= keepAliveTickRate)
        {
            lastKeepAlive = 0f;
            Broadcast(new NetKeepAlive());
        }
        lastKeepAlive += Time.deltaTime;
    }

    private void SpawnPowerUp()
    {
        if( !shouldSpawnPowerUps)
        {
            return;
        }
        if( lastPowerUpSpawnTimeElapsed >= thisPowerUpSpawnTime )
        {
            Broadcast( new NetInstantiatePowerUp( PowerUpSpawnManager.Instance.GetRandomSpawnPosition() , PowerUpSpawnManager.Instance.GetRandomPowerUp() , AllotPowerUpID() ) );
            lastPowerUpSpawnTimeElapsed = 0;
            thisPowerUpSpawnTime = GetRandomFloat( minPowerUpSpawnTime , maxPowerUpSpawnTime );
        }
        lastPowerUpSpawnTimeElapsed += Time.deltaTime;
    }

    private void RemovePlayerWithConnection( NetworkConnection cnn)
    {
        for( int i = 0 ; i < players.Count ; i++)
        {
            if( players[i].connection == cnn )
            {
                players.RemoveAt(i);
                return;
            }
        }
    }

    private void Update()
    {
        if(!isActive)
        {
            return;
        }

        driver.ScheduleUpdate().Complete();
        KeepAlive();
        CleanupConnections();
        AcceptNewConnection();
        UpdateMessagePump();
        SpawnPowerUp();
    }

    private void CleanupConnections()
    {
        for ( int i = 0 ; i < connections.Length ; i++ )
        {
            if( !connections[i].IsCreated )
            {
                connections.RemoveAtSwapBack(i);
                MasterServer.Instance.UpdateNumOfPlayers( port, connections.Length);
                // Debug.Log("Sending new list with length " + players.Count);
                Broadcast( new NetPlayerInfoList( players ) );
                if ( players.Count > 0 )
                SendToClient( new NetBecomeMasterClient() , players[0].connection );
                --i;
            }
        }
    }

    private void AcceptNewConnection()
    {
        NetworkConnection c;
        while(  (c = driver.Accept() )!= default(NetworkConnection) )
        {
            connections.Add(c);
            MasterServer.Instance.UpdateNumOfPlayers( port, connections.Length);
        }
    }

    public void UpdateMessagePump()
    {
        DataStreamReader reader;
        for( int i = 0 ; i < connections.Length ; i++)
        {
            NetworkEvent.Type cmd;
            while( ( cmd = driver.PopEventForConnection(connections[i]  , out reader) ) != NetworkEvent.Type.Empty )
            {
                if ( cmd == NetworkEvent.Type.Connect)
                {
                    
                }
                else if( cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData( reader , port , connections[i] , true);
                }
                else if( cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("SERVER : Disconnected client" );
                    RemovePlayerWithConnection(connections[i]);
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    public void SendToClient(NetMessage msg  , NetworkConnection cnn)
    {
        DataStreamWriter writer;
        driver.BeginSend(cnn , out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    public void Broadcast(NetMessage msg)
    {
        for( int i = 0 ; i < connections.Length ; i ++)
        {
            SendToClient(msg , connections[i]);
        }
    }

    public float GetRandomFloat(float minimum, float maximum)
    { 
        return ( ( (float)random.NextDouble() )* (maximum - minimum) + minimum) ;
    }

    private void RegisterEvents()
    {
        NetUtility.S_PLAYER_INFO += OnReceivedPlayerInfo;
        NetUtility.S_LOAD_GAME += OnReceivedLoadGame;
        NetUtility.S_GAME_LOADED += OnReceivedGameLoaded;
        NetUtility.S_HORIZONTAL_INPUT += OnReceivedHorizontalInput;
        NetUtility.S_JUMP_INPUT += OnReceivedJumpInput;
        NetUtility.S_SYNC_POSITION += OnReceivedSyncPosition;
        NetUtility.S_AIM_INPUT += OnReceivedAimInput;
        NetUtility.S_PROJECTILE_HIT += OnReceivedProjectileHit;
        NetUtility.S_RESPAWN += OnReceivedRespawn;
        NetUtility.S_UPDATE_SCORE += OnReceivedUpdateScore;
        NetUtility.S_APPLY_POWER_UP += OnReceivedApplyPowerUp;
    }

    private void UnregisterEvents()
    {
        NetUtility.S_PLAYER_INFO -= OnReceivedPlayerInfo;
        NetUtility.S_LOAD_GAME -= OnReceivedLoadGame;
        NetUtility.S_GAME_LOADED -= OnReceivedGameLoaded;
        NetUtility.S_HORIZONTAL_INPUT -= OnReceivedHorizontalInput;
        NetUtility.S_JUMP_INPUT -= OnReceivedJumpInput;
        NetUtility.S_SYNC_POSITION -= OnReceivedSyncPosition;
        NetUtility.S_AIM_INPUT -= OnReceivedAimInput;
        NetUtility.S_PROJECTILE_HIT -= OnReceivedProjectileHit;
        NetUtility.S_RESPAWN -= OnReceivedRespawn;
        NetUtility.S_UPDATE_SCORE -= OnReceivedUpdateScore;
        NetUtility.S_APPLY_POWER_UP -= OnReceivedApplyPowerUp;
    }  

    private int AllotPlayerID()
    {
        int id = random.Next(1000 , 9999);
        for ( int i = 0 ; i < players.Count ; i++)
        {
            if( id == players[i].PlayerID )
            {
                id = AllotPlayerID();
                break;
            }
        }
        return id;
    }

    private int AllotPowerUpID()
    {
        int id = random.Next(1000 , 9999);
        for ( int i = 0 ; i < allottedPowerUpIDs.Count ; i++)
        {
            if( id == allottedPowerUpIDs[i] )
            {
                id = AllotPowerUpID();
                break;
            }
        }
        return id;
    }

    private void OnReceivedPlayerInfo(NetMessage msg , ushort _port , NetworkConnection cnn)
    {
        if( _port != port )
            return;

        NetPlayerInfo infoMsg = msg as NetPlayerInfo;

        // Send the Allotted PlayerId to the client for future references
        int playerID = AllotPlayerID();
        SendToClient( new NetWelcome(playerID) , cnn ) ;

        // Send the updated List to the clients
        PlayerInfo info = new PlayerInfo( infoMsg.PlayerInfo.nickName , playerID , cnn);
        players.Add(info);
        // Debug.Log("SERVER : received Player info. Currently there are " + players.Count);
        Broadcast( new NetPlayerInfoList(players) );

        // if it is the first player to join, make it the master client.
        if(players.Count == 1)
        {
            SendToClient( new NetBecomeMasterClient() , cnn );
        }
    }

    private void OnReceivedLoadGame( NetMessage msg , ushort _port , NetworkConnection cnn )
    {
        if( _port != port )
            return;

        Broadcast( msg );
    }
    
    private void OnReceivedGameLoaded( NetMessage msg , ushort _port , NetworkConnection cnn)
    {
        if( _port != port )
            return;

        Debug.Log("GameLoaded");
        playersLoaded++ ;
        if( playersLoaded == players.Count)
        {
            for( int i = 0; i < players.Count ; i++)
            {
                Broadcast( new NetInstantiate(players[i]) );
            }
            Broadcast( new NetStartGame() );
            shouldSpawnPowerUps = true;
            thisPowerUpSpawnTime = GetRandomFloat( minPowerUpSpawnTime , maxPowerUpSpawnTime );
        }
    }

    private void OnReceivedHorizontalInput( NetMessage msg , ushort _port , NetworkConnection cnn)
    {
        if( _port != port )
            return;

        Broadcast( msg );
    }
    
    private void OnReceivedJumpInput( NetMessage msg , ushort _port , NetworkConnection cnn)
    {
        if( _port != port )
            return;

        Broadcast( msg );
    }

    private void OnReceivedSyncPosition( NetMessage msg , ushort _port , NetworkConnection cnn)
    {
        if( _port != port )
            return;

        Broadcast( msg );
    }

    private void OnReceivedAimInput( NetMessage msg , ushort _port , NetworkConnection cnn)
    {
        if( _port != port )
            return;

        Broadcast( msg );
    }

    private void OnReceivedProjectileHit( NetMessage msg , ushort _port , NetworkConnection cnn)
    {
        if( _port != port )
            return;

        // Debug.Log("ANC");
        Broadcast( msg );
    }

    private void OnReceivedRespawn( NetMessage msg , ushort _port , NetworkConnection cnn )
    {
        if( _port != port )
            return;

        NetRespawn respawnMsg = msg as NetRespawn;
        Broadcast( new NetRespawn( respawnMsg.playerID , PlayerSpawnManager.Instance.GetRandomSpawnPosition() ) );
    }

    private void OnReceivedUpdateScore( NetMessage msg , ushort _port , NetworkConnection cnn )
    {      
        if( _port != port )
            return;

        NetUpdateScore updateScoreMsg = msg as NetUpdateScore;
        for( int i = 0 ; i < players.Count ; i++ )
        {
            if( players[i].PlayerID == updateScoreMsg.playerID )
            {
                players[i].score += updateScoreMsg.score;
                Debug.Log("SERVER" + players[i].score.ToString());
                Broadcast( msg );
            }
        } 

        Broadcast( new NetPlayerInfoList(players) );
    }

    private void OnReceivedApplyPowerUp( NetMessage msg , ushort _port , NetworkConnection cnn)
    {
        if( _port != port )
            return;

        for ( int i = 0; i < allottedPowerUpIDs.Count; i++)
        {
            if( allottedPowerUpIDs[i] == ( (NetApplyPowerUp)msg ).powerUpInstanceID )
            {
                allottedPowerUpIDs.RemoveAt(i);
            }
        }
        Broadcast( msg );
    }

    private void OnReceivedDisconnect( NetMessage msg, ushort _port, NetworkConnection cnn)
    {
        for( int i = 0 ; i < connections.Length ; i++)
        {
            if( connections[i] == cnn)
            {
                Debug.Log("SERVER : Disconnected client" );
                RemovePlayerWithConnection(connections[i]);
                connections[i] = default(NetworkConnection);
                driver.Disconnect(connections[i]);                
            }
        }
    }

}
