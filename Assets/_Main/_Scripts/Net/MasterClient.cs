using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System;
using System.Collections.Generic;

public class MasterClient : MonoBehaviour
{
    public static MasterClient Instance;

    public ushort port = 8008;

    private NetworkDriver driver;
    private NetworkConnection connection;
    private bool isActive = false;

    [SerializeField] private GameObject clientPrefab;

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
        Application.targetFrameRate = 45;
        RegisterEvents();
    }


    public void Init() //string ip , int port)
    {
        string ip = MainMenu.Instance.ipAdressTextField.value;
        ushort port = 8008;
        isActive = true;
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.Parse( ip , port);
        endpoint.Port = port;
        connection = driver.Connect(endpoint);
    }

    private void Update()
    {
        if(!isActive)
        {
            return;
        }
        driver.ScheduleUpdate().Complete();
        CheckAlive();
        UpdateMessagePump();
    }

    public void Shutdown()
    {
        if(isActive)
        {
            // Debug.Log("CLIENT : Shutting down");
            driver.Dispose();
            connection = default(NetworkConnection);
            isActive = false;
        }
    }

    public void OnDestroy()
    {
        connection.Disconnect(driver);
        Shutdown();
        UnregisterEvents();
    }

    private void CheckAlive()
    {
        if ( !connection.IsCreated && isActive)
        {
            // Debug.Log("CLIENT : Something Went wrong, lost conenction to server");
            connection = default(NetworkConnection);
            // Shutdown();
        }
    }

    public void UpdateMessagePump()
    {
        DataStreamReader reader;

        NetworkEvent.Type cmd;
        while( ( cmd = connection.PopEvent( driver , out reader ) ) != NetworkEvent.Type.Empty  )
        {
            if ( cmd == NetworkEvent.Type.Connect)
            {
                MenuManager.Instance.OpenMenu("PlayMenu");
            }
            else if( cmd == NetworkEvent.Type.Data)
            {
                NetUtility.OnData(reader , port , connection, false );
            }
            else if( cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client : Disconnected client" );
                // Remove the player from the list                                               :: TODO
            }

        }
    }
  
    public void JoinRoom( ushort _port )
    {
        Client client = Instantiate( clientPrefab ).GetComponent<Client>();
        client.port = _port;
        client.Init();
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;

        driver.BeginSend(connection , out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    public void RegisterEvents()
    {
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
        NetUtility.C_ROOM_INFO_LIST += OnReceivedRoomInfoList;
        NetUtility.C_JOIN_ROOM += OnReceivedJoinRoom;
    }

    public void UnregisterEvents()
    {
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
        NetUtility.C_ROOM_INFO_LIST -= OnReceivedRoomInfoList;
        NetUtility.C_JOIN_ROOM -= OnReceivedJoinRoom;
    }

    private void OnKeepAlive( NetMessage msg)
    {
        SendToServer(msg);
    }

    private void OnReceivedRoomInfoList( NetMessage msg )
    {
        NetRoomInfoList roomInfoListMsg = msg as NetRoomInfoList;
        MenuManager.Instance.UpdateRoomInfoList( roomInfoListMsg.roomInfos );
    }

    private void OnReceivedJoinRoom( NetMessage msg)
    {
        NetJoinRoom joinRoomMsg = msg as NetJoinRoom;
        MasterClient.Instance.JoinRoom(joinRoomMsg.port);
        MenuManager.Instance.OpenMenu("Loading");
    }
}
