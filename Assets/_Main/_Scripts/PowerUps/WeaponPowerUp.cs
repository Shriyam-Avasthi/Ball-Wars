using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPowerUp : PowerUp
{
    [SerializeField] private GameObject powerUpCollectionPrefab;

    private void Awake()
    {
        Type = PowerUpType.WeaponPowerUp;
    }

    public override void OnTriggerEnter( Collider other )
    {
        if( other.gameObject.CompareTag("Player") )
        {
            if( other.gameObject.GetComponent<NetworkPlayer>().isMine )
            {
                int id = other.gameObject.GetComponent<NetworkPlayer>().playerInfo.PlayerID;
                Client.Instance.SendToServer( new NetApplyPowerUp( this.Type , id , this.InstanceID)  );

            }
        }
    }

    private void OnDestroy()
    {
        GameObject obj = Instantiate(powerUpCollectionPrefab , this.transform.position, Quaternion.identity);
        Destroy( obj , 1f );
    }

    public override void SetUp(int id)
    {
        InstanceID = id;
    }
}
