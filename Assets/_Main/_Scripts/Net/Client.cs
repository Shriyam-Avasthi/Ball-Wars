using UnityEngine;
using Unity.Networking.Transport;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class Client : MonoBehaviour
{
    public static Client Instance;
    public int MyPlayerID { private set; get; }
    public ushort port = 8008;
    public bool isMasterClient = false;

    private Action connectionDropped;
    private bool isLoadingGame = false;
    private NetworkDriver driver;
    private NetworkConnection connection;
    private bool isActive = false;


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
        RegisterEvents();
    }


    public void Init() //string ip , int port)
    {
        string ip = MainMenu.Instance.ipAdressTextField.value;
        isActive = true;
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(ip, port);
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
        if(connection != default(NetworkConnection))
        driver.Disconnect(connection);
        Shutdown();
        UnregisterEvents();
    }

    public void DisconnectFromServer()
    {
        StartCoroutine("Disconnect");
    }

    private IEnumerator Disconnect()
    {
        driver.Disconnect(connection);
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }

    private void CheckAlive()
    {
        if ( !connection.IsCreated && isActive)
        {
            // Debug.Log("CLIENT : Something Went wrong, lost conenction to server");
            connectionDropped?.Invoke();
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
                MenuManager.Instance.OpenMenu("Lobby");
                SendToServer( new NetPlayerInfo( new PlayerInfo(PlayMenu.Instance.playerNameField.text , 0) ) );
            }
            else if( cmd == NetworkEvent.Type.Data)
            {
                NetUtility.OnData(reader , port ,connection, false );
            }
            else if( cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client : Disconnected client" );
                // Remove the player from the list                                               :: TODO
            }

        }
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
        connectionDropped += OnConnectionDropped;
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
        NetUtility.C_PLAYER_INFO_LIST += OnReceivedPlayerInfoList;
        NetUtility.C_START_GAME += OnReceivedStartGame;
        NetUtility.C_LOAD_GAME += OnReceivedLoadGame;
        NetUtility.C_BECOME_MASTER_CLIENT += OnReceivedBecomeMasterClient;
        NetUtility.C_NET_WELCOME += OnReceivedNetWelcome;
        NetUtility.C_INSTANTIATE += OnReceivedInstantiate;
        NetUtility.C_UPDATE_SCORE += OnReceivedUpdateScore;
        NetUtility.C_PROJECTILE_HIT += OnReceivedProjectileHit;
        NetUtility.C_RESPAWN += OnReceivedRespawn;
        NetUtility.C_INSTANTIATE_POWER_UP += OnReceivedInstantiatePowerUp;
        NetUtility.C_APPLY_POWER_UP += OnReceivedApplyPowerUp;
    }
    public void UnregisterEvents()
    {
        connectionDropped -= OnConnectionDropped;
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
        NetUtility.C_PLAYER_INFO_LIST -= OnReceivedPlayerInfoList;
        NetUtility.C_START_GAME += OnReceivedStartGame;
        NetUtility.C_LOAD_GAME -= OnReceivedLoadGame;
        NetUtility.C_BECOME_MASTER_CLIENT -= OnReceivedBecomeMasterClient;
        NetUtility.C_NET_WELCOME -= OnReceivedNetWelcome;
        NetUtility.C_INSTANTIATE -= OnReceivedInstantiate;
        NetUtility.C_UPDATE_SCORE -= OnReceivedUpdateScore;
        NetUtility.C_PROJECTILE_HIT -= OnReceivedProjectileHit;
        NetUtility.C_RESPAWN -= OnReceivedRespawn;
        NetUtility.C_INSTANTIATE_POWER_UP -= OnReceivedInstantiatePowerUp;
        NetUtility.C_APPLY_POWER_UP -= OnReceivedApplyPowerUp;
    }

    private void OnConnectionDropped()
    {
        Shutdown();
        MenuManager.Instance.OpenMenu("MainMenu");
    }

    private void OnKeepAlive( NetMessage msg)
    {
        SendToServer(msg);
    }

    private void OnReceivedPlayerInfoList(NetMessage msg)
    {
        List<PlayerInfo> infos = ( (NetPlayerInfoList)msg ).PlayerInfoList;
        MenuManager.Instance.Lobby_UpdatePlayerList(infos);
    }

    private void OnReceivedLoadGame(NetMessage msg)
    {
        if( !isLoadingGame)
        {
            MenuManager.Instance.OpenMenu("Loading");
            StartCoroutine("StartGame");
        }
    }

    private void OnReceivedStartGame( NetMessage msg)
    {
        MenuManager.Instance.OpenMenu("InGameUI");
    }

    private IEnumerator StartGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1 , LoadSceneMode.Additive );
        isLoadingGame = true;
        while( !asyncLoad.isDone)
        {
            yield return null;
        }
        MenuManager.Instance.OpenMenu("WaitScreen");
        AudioManager.Instance.PlayInGameMusic();
        isLoadingGame = false;
        SendToServer( new NetGameLoaded() );
    }

    private void OnReceivedBecomeMasterClient( NetMessage msg )
    {
        isMasterClient = true;
        MenuManager.Instance.ActivateMasterMenu();
    }

    private void OnReceivedNetWelcome( NetMessage msg )
    {
        NetWelcome welcomeMsg = msg as NetWelcome;
        MyPlayerID = welcomeMsg.PlayerId;
    }

    private void OnReceivedInstantiate( NetMessage msg )
    {
        NetInstantiate InstantiateMsg = msg as NetInstantiate;
        NetworkPlayerManager.Instance.InstantiateNetworkPlayer(InstantiateMsg.playerInfo);
    }

    private void OnReceivedProjectileHit( NetMessage msg )
    {
        NetProjectileHit projectileHitMsg = msg as NetProjectileHit;
        NetworkPlayerManager.Instance.ApplyDamageToPlayerWithID( projectileHitMsg.hitterID ,projectileHitMsg.playerID , projectileHitMsg.damage , projectileHitMsg.hitForce ) ;
        // Debug.Log("Hit" + projectileHitMsg.hitterID.ToString() + projectileHitMsg.playerID.ToString());
    }

    private void OnReceivedUpdateScore( NetMessage msg )
    {
        NetUpdateScore updateScoreMsg = msg as NetUpdateScore;
        MenuManager.Instance.UpdatePlayerScore( updateScoreMsg.playerID , updateScoreMsg.score );
    }

    private void OnReceivedRespawn( NetMessage msg )
    {
        NetRespawn respawnMsg = msg as NetRespawn;
        NetworkPlayerManager.Instance.RespawnPlayerWithID( respawnMsg.playerID , respawnMsg.respawnPosition);
    }

    private void OnReceivedInstantiatePowerUp( NetMessage msg)
    {
        NetInstantiatePowerUp powerUpInstantiateMsg = msg as NetInstantiatePowerUp;
        PowerUpManager.Instance.InstantiatePowerUp( powerUpInstantiateMsg.position , powerUpInstantiateMsg.powerUpInfoID , powerUpInstantiateMsg.powerUpInstanceID );
    }

    private void OnReceivedApplyPowerUp( NetMessage msg )
    {
        PowerUpManager.Instance.ApplyPowerUp( msg );
    }  
}
