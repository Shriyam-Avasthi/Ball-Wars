using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerManager : MonoBehaviour
{
    public static NetworkPlayerManager Instance;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float respawnWait ;
    [SerializeField] private int killPoints; 

    private List<NetworkPlayer> players;

    private void Awake()
    {
        if( Instance != null)
        {
            Destroy( Instance.gameObject );
        }
        Instance = this;
        players = new List<NetworkPlayer>();
    }

    public void InstantiateNetworkPlayer( PlayerInfo info)
    {
        GameObject player = Instantiate( playerPrefab , new Vector3( 0f , 3f , -1f ) , Quaternion.identity ); 
        player.GetComponent<NetworkPlayer>().playerInfo = info;
        players.Add( player.GetComponent<NetworkPlayer>() );
    }

    public void ApplyPowerUpToPlayerWithID( int playerID, PowerUp powerUp)
    {
        for( int i = 0; i < players.Count; i++)
        {
            if( players[i].playerInfo.PlayerID == playerID)
            {
                var x = players[i].powerUpObjects;
                for( int j = 0 ; j < x.Length; j++)
                {
                    IPowerUpable powerUpable = x[j].GetComponentInChildren<IPowerUpable>();
                    if( powerUpable.PowerUpTriggerType == powerUp.Type )
                    {
                        if( powerUp.Type == PowerUpType.WeaponPowerUp)
                        {
                            powerUpable.PowerUp( (WeaponPowerUpInfo)powerUp.itemInfo );
                        }
                    }
                }
                break;
            }

        }
    }

    public void ApplyDamageToPlayerWithID( int hitterID ,int id , int damage , Vector3 force )
    {
        GameObject player = null ; 
        float weaknessMultiplier;

        for( int i = 0 ; i < players.Count ; i++)
        {
            if( players[i].playerInfo.PlayerID == id )
            {
                player = players[i].gameObject;
                players[i].hitterID = hitterID;
                players[i].hitDirection = force.normalized;
            }
        }

        IDamageable damageable = player.GetComponent<IDamageable>();
        damageable.ApplyDamage( damage );

        if(damageable.HP != 0)
        {
            weaknessMultiplier = damageable.maxHP / damageable.HP ;
        }
        else
        {
            if( id == Client.Instance.MyPlayerID )
            {
                StartCoroutine("WaitForRespawn" , respawnWait );
            }
            weaknessMultiplier = 1;
        }
        player.GetComponent<Rigidbody>().AddForce( force * weaknessMultiplier , ForceMode.Impulse );
    }

    public void RespawnPlayerWithID( int id , Vector3 position )
    {
        for( int i = 0 ; i < players.Count ; i++)
        {
            if( players[i].playerInfo.PlayerID == id )
            {
                players[i].gameObject.SetActive( true );
                players[i].gameObject.transform.position = position;
                var damageable = players[i].gameObject.GetComponent<IDamageable>();
                damageable.HP = damageable.maxHP;
            }
        }
    }

    public void StartRespawn()
    {
        StartCoroutine("WaitForRespawn" , respawnWait );
    }

    private IEnumerator WaitForRespawn( float time )
    {
        float timeElapsed = 0f;
        while( true )
        {
            if( timeElapsed >= time )
            {
                break;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        Client.Instance.SendToServer( new NetRespawn( Client.Instance.MyPlayerID , Vector3.zero ) ) ;
    }
    
    public void AwardPointsToPlayerWithID( int id )
    {
        Client.Instance.SendToServer( new NetUpdateScore( id , killPoints ) );
    }


}
