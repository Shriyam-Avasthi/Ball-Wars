using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PowerUpManager : MonoBehaviour
{
    [SerializeField] List<GameObject> powerUpPrefabs = new List<GameObject>();

    public static PowerUpManager Instance;

    private List<PowerUp> powerUps = new List<PowerUp>();
    // private System.Random random;

    #region Singleton 
    
    private void Awake()
    {
        if( Instance != null )
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        // random = new System.Random();
    }
    #endregion
    
    public void InstantiatePowerUp( Vector3 position , int powerUpInfoID, int powerUpInstanceID )
    {
        for( int i = 0 ; i < powerUpPrefabs.Count ; i++)
        {
            PowerUpInfo powerUpInfo = (PowerUpInfo)powerUpPrefabs[i].GetComponent<PowerUp>().itemInfo;

            if( powerUpInfo.ID == powerUpInfoID )
            {
                GameObject powerUp = Instantiate( powerUpPrefabs[i] , position , Quaternion.identity );
                powerUp.GetComponent<PowerUp>().SetUp( powerUpInstanceID );
                powerUps.Add( powerUp.GetComponent<PowerUp>() );
                AudioManager.Instance.PlayPowerUpSpawnSfx( position , powerUp.GetComponent<PowerUp>().Type);
                break;
            }
        }
    }

    public void ApplyPowerUp(NetMessage msg)
    {
        NetApplyPowerUp powerUpMsg = msg as NetApplyPowerUp;
        for( int i = 0 ; i < powerUps.Count ; i++)
        {
            if( powerUps[i].InstanceID == powerUpMsg.powerUpInstanceID)
            {
                NetworkPlayerManager.Instance.ApplyPowerUpToPlayerWithID(powerUpMsg.targetPlayerID , powerUps[i] );
                AudioManager.Instance.PlayPowerUpCollectSfx( powerUps[i].gameObject.transform.position , powerUps[i].Type);
                Destroy( powerUps[i].gameObject);
            }
        }
    }

    private void RegisterEvents()
    {
        NetUtility.C_APPLY_POWER_UP += ApplyPowerUp;
    }

    private void UnregisterEvents()
    {
        NetUtility.C_APPLY_POWER_UP -= ApplyPowerUp;
    }
}
