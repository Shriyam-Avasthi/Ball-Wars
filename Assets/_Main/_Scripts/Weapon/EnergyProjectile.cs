using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyProjectile : Projectile, IPowerUpable
{
    [SerializeField] private GameObject hitImpact;
    [SerializeField] private GameObject powerUpEffect;

    public bool isMine;
    public PowerUpType PowerUpTriggerType{get;}

    private float maxCharge;
    private float thresholdChargingTime;
    private float chargingTime;
    private float chargedDeltaRadius;
    private  Coroutine chargeCoroutine = null;
    private Rigidbody rb;
    private ProjectileInfo projectileInfo;
    private float deltaChargePerSec;
    private float deltaRadiusPerSec;
    private float timeElapsed = 0f;
    private bool poweredUp = false;
    private int damagePerCharge;

    public bool isLaunched = false;

    [HideInInspector] public float currentCharge = 1f;
    [HideInInspector] public bool isCharging = false;

    private void Awake()
    {
        projectileInfo = ((ProjectileInfo)itemInfo);
        maxCharge = projectileInfo.maxCharge;
        thresholdChargingTime = projectileInfo.thresholdChargingTime;
        chargingTime = projectileInfo.chargingTime;
        chargedDeltaRadius = projectileInfo.chargedDeltaRadius;
        deltaChargePerSec = maxCharge / chargingTime;
        deltaRadiusPerSec = chargedDeltaRadius / chargingTime;
        damagePerCharge = projectileInfo.damagePerCharge;
        rb = this.GetComponent<Rigidbody>();
    }

    private IEnumerator Charge()
    {
        float timeElapsed = 0f;
        currentCharge = 1f;
        while(currentCharge < maxCharge)
        {
            if (timeElapsed > thresholdChargingTime)
            {
                currentCharge += deltaChargePerSec * Time.deltaTime;
                this.transform.localScale += new Vector3(deltaRadiusPerSec , deltaRadiusPerSec, deltaRadiusPerSec) * Time.deltaTime;
                if( poweredUp )
                {
                    powerUpEffect.transform.localScale += new Vector3( deltaRadiusPerSec , deltaRadiusPerSec , deltaRadiusPerSec) * Time.deltaTime;
                }
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }  
        if( currentCharge > maxCharge)  
        {
            currentCharge = maxCharge;
        }
    }
    public void StartCharge()
    {
        isCharging = true;
        chargeCoroutine = StartCoroutine(Charge());
    }

    public void StopCharge()
    {   
        if( chargeCoroutine != null)
        {
            isCharging = false;
            StopCoroutine(chargeCoroutine);
        }
    }

    private IEnumerator Move(float speed , Vector3 dir)
    {
        rb.velocity = speed * dir.normalized;
        while(timeElapsed < projectileInfo.lifeTime)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    public override void Launch( Vector3 dir)
    {
        timeElapsed = 0f;
        isLaunched = true;
        StartCoroutine( Move( projectileInfo.speed , dir) );
    }

    public override void OnTriggerStay(Collider other)
    {
        int damage =  (int)( damagePerCharge *  currentCharge ) ;
        Vector3 force = projectileInfo.hitImpulse * rb.velocity.normalized * currentCharge;

        if(!isCharging)
        {
            // if( isMine )
            // {
            if( other.gameObject.GetComponent<NetworkPlayer>() )
            {
                if( other.gameObject.CompareTag("Player") && isMine && !other.gameObject.GetComponent<NetworkPlayer>().isMine )
                {
                    // Debug.Log( "EnergyProjectile" +Client.Instance.MyPlayerID.ToString() +other.gameObject.GetComponent<NetworkPlayer>().playerInfo.PlayerID.ToString());
                    Client.Instance.SendToServer( new NetProjectileHit( Client.Instance.MyPlayerID , other.gameObject.GetComponent<NetworkPlayer>().playerInfo.PlayerID , damage , force ) );
                }
                if( ! (isMine && other.gameObject.GetComponent<NetworkPlayer>().isMine) )
                {
                    Instantiate( hitImpact , this.transform.position , Quaternion.identity);             
                    // }
                    Destroy(this.gameObject);
                }

            }
            else
            {
                Instantiate( hitImpact , this.transform.position , Quaternion.identity);      
                AudioManager.Instance.PlayEnergyProjectileExplosionSfx(this.transform.position , (currentCharge / maxCharge) , poweredUp );       
                // }
                Destroy(this.gameObject);
            }
            
            
        }
    }

    public void PowerUp( PowerUpInfo powerUpInfo)
    {                           
        WeaponPowerUpInfo weaponPowerUpInfo = powerUpInfo as WeaponPowerUpInfo;         
        damagePerCharge += weaponPowerUpInfo.deltaDamagePerCharge;
        chargedDeltaRadius += weaponPowerUpInfo.deltaMaxSize;
        maxCharge += weaponPowerUpInfo.deltaMaxCharge;
        chargingTime += weaponPowerUpInfo.deltaChargingTime;

        powerUpEffect.SetActive(true);
        poweredUp = true;
    }

}
