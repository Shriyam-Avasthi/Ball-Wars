using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System;
using System.Collections.Generic;

public class MasterServer : MonoBehaviour
{
    public static MasterServer Instance;

    public ushort port = 8008;

    [SerializeField] private GameObject serverPrefab;
    [SerializeField] private ushort startingServerPort;

    private NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    private bool isActive = false;
    private const float keepAliveTickRate = 20.0f;
    private float lastKeepAlive;

    private List<RoomInfo> roomInfos = new List<RoomInfo>();

    private void Awake()
    {
        if( Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
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
    }

    private void CleanupConnections()
    {
        for ( int i = 0 ; i < connections.Length ; i++ )
        {
            if( !connections[i].IsCreated )
            {
                connections.RemoveAtSwapBack(i);
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
            SendToClient( new NetRoomInfoList(roomInfos) , c );
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
                    SendToClient( new NetRoomInfoList( roomInfos ) , connections[i] );
                }
                else if( cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData( reader , port , connections[i] , true);
                }
                else if( cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("SERVER : Disconnected client" );
                    connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    public void SendToClient(NetMessage msg , NetworkConnection cnn)
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

    public void UpdateNumOfPlayers(ushort port, int numOfPlayers)
    {
        for( int i = 0 ; i < roomInfos.Count ; i++)
        {
            if( roomInfos[i].port == port )
            {
                roomInfos[i].numOfPlayers = numOfPlayers;
                Broadcast( new NetRoomInfoList(roomInfos) );
                Debug.Log(numOfPlayers);
            }
        }
    }

    private ushort AssignPort()
    {
        return (ushort)(startingServerPort + (roomInfos.Count) );
    }

    private void RegisterEvents()
    {
        NetUtility.S_CREATE_ROOM += OnReceivedCreateRoom;
        // NetUtility.S_FETCH_ROOM_INFO += OnReceivedFetchRoomInfo;
    }

    private void UnregisterEvents()
    {
        NetUtility.S_CREATE_ROOM -= OnReceivedCreateRoom;
        // NetUtility.S_FETCH_ROOM_INFO -= OnReceivedFetchRoomInfo;
    }

    private void OnReceivedCreateRoom( NetMessage msg , ushort _port , NetworkConnection cnn)
    {
        NetCreateRoom createRoomMsg = msg as NetCreateRoom;
        Server server = Instantiate(serverPrefab).GetComponent<Server>();
        ushort port = AssignPort();
        server.name = createRoomMsg.name;
        server.port = port;
        RoomInfo roomInfo = new RoomInfo( port , createRoomMsg.name);
        roomInfos.Add( roomInfo);
        server.Init();
        Broadcast( new NetRoomInfoList(roomInfos) );
        SendToClient( new NetJoinRoom(port) , cnn);
    }

    // private void OnReceivedFetchRoomInfo(NetMessage msg , NetworkConnection cnn)
    // {
    //     SendToClient()
    // }
}
