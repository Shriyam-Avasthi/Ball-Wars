using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class EnergyCanon : Weapon, IPowerUpable
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject chargeShockWaves;
    [SerializeField] private GameObject chargeSphere;
    [SerializeField] private GameObject chargeAura;
    [SerializeField] private GameObject chargeEffect;

    public PowerUpType PowerUpTriggerType{get;}
   
    private WeaponPowerUpInfo weaponPowerUpInfo;
    private bool isCharging;
    private bool isMine;
    private float minFireWait;
    private float fireTimeElapsed;
    private GameObject projectileInstance;
    private PlayerInputActions playerInputActions;
    private WeaponInfo weaponInfo;
    private Vector2 prev_aimInput;
    private bool isPoweredUp = false;
    private Coroutine powerUpCoroutine = null;

    private void OnDisable() {
        playerInputActions.Player.Disable();
    }

    private void OnEnable()
    {
        if( isMine)
        {
            playerInputActions.Player.Enable();  
        }
    }

    private void OnDestroy()
    {
        if( !isMine )
        {
            UnregisterEvents();
        }
    }

    public override void Aim()
    {
        Vector2 aimVector;
        if( isMine )
        {
            aimVector = playerInputActions.Player.Aim.ReadValue<Vector2>();
            if( prev_aimInput != aimVector)
            {
                Client.Instance.SendToServer( new NetAimInput(Client.Instance.MyPlayerID , aimVector) );
                prev_aimInput = aimVector;
            }
        }

        else
        {
            aimVector = prev_aimInput;
        }
        weaponPivot.rotation = Quaternion.identity;
        Vector2 currentWeaponVector = new Vector2(this.transform.localPosition.x , this.transform.localPosition.y);
        float angle = Vector2.SignedAngle(currentWeaponVector , aimVector);
        this.transform.RotateAround( weaponPivot.position , Vector3.forward , weaponInfo.weaponRotateSpeed * angle * Time.deltaTime);
        if (projectileInstance != null && isCharging)
        {
            projectileInstance.transform.position = muzzle.transform.position;
        }
    }

    public override void Use()
    {
        Vector2 aimVector;
        if( isMine )
        {
            aimVector =  playerInputActions.Player.Aim.ReadValue<Vector2>(); 
            if ( aimVector != prev_aimInput)
            {
                Client.Instance.SendToServer( new NetAimInput( Client.Instance.MyPlayerID , aimVector ));
                prev_aimInput = aimVector;
            }
        }
        else
        {
            aimVector = prev_aimInput;
        }
        if(aimVector.magnitude > 0.8 && !isCharging && fireTimeElapsed >= minFireWait )
        {
            isCharging = true;
            projectileInstance = Instantiate(projectile , muzzle.position , Quaternion.identity) ;
            if( isPoweredUp)
            {
                projectileInstance.GetComponent<EnergyProjectile>().PowerUp(weaponPowerUpInfo);
            }
            projectileInstance.GetComponent<EnergyProjectile>().isMine = isMine;
            projectileInstance.GetComponent<EnergyProjectile>().StartCharge();
            fireTimeElapsed = 0f;
        }
        else if (aimVector.magnitude == 0 && isCharging)
        {
            Fire();
        }
        fireTimeElapsed += Time.deltaTime;
    }

    public void Fire()
    { 
        projectileInstance.GetComponent<EnergyProjectile>().StopCharge();
        projectileInstance.GetComponent<EnergyProjectile>().Launch( this.transform.up);
        isCharging = false;
        this.GetComponentInParent<Rigidbody>().AddForce( projectileInstance.GetComponent<EnergyProjectile>().currentCharge * weaponInfo.recoil * ( - this.transform.up) , ForceMode.Impulse);
    }   

    public void PowerUp( PowerUpInfo powerUpInfo )
    {
        if( powerUpCoroutine != null )
        {
            StopCoroutine(powerUpCoroutine);
        }
        weaponPowerUpInfo = powerUpInfo as WeaponPowerUpInfo;
        isPoweredUp = true;
        powerUpCoroutine = StartCoroutine(KeepPowerUp());
    }

    private IEnumerator KeepPowerUp()
    {
        float timeElapsed = 0f;
        while( timeElapsed < weaponPowerUpInfo.effectLifetime )
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        isPoweredUp = false;
        weaponPowerUpInfo = null;
        powerUpCoroutine = null;
    }

    private void Start()
    {
        playerInputActions = new PlayerInputActions();
        weaponInfo = (WeaponInfo)itemInfo;
        minFireWait = 1 / weaponInfo.fireRate;
        NetworkPlayer player = GetComponentInParent<NetworkPlayer>();
        if( !player.isMine )
        {
            playerInputActions.Player.Disable();  
            isMine = false;
            RegisterEvents();
        }
        else
        {
            playerInputActions.Player.Enable();  
            isMine = true;
        }
    }

    private void RegisterEvents()
    {
        NetUtility.C_AIM_INPUT += OnReceivedAimInput;
    }

    private void UnregisterEvents()
    {
        NetUtility.C_AIM_INPUT -= OnReceivedAimInput;
    }

    private void OnReceivedAimInput( NetMessage msg )
    {
        NetAimInput aimInputMsg = msg as NetAimInput;
        if( GetComponentInParent<NetworkPlayer>()?.playerInfo.PlayerID == aimInputMsg.playerID )
        {
            prev_aimInput = aimInputMsg.aimInput;
        }
    }

    private void FixedUpdate()
    {
        Aim();
        Use();
        chargeShockWaves.SetActive(isCharging && GetComponentInParent<PlayerController>().isGrounded);
        if( projectileInstance != null)
        {
            EnergyProjectile proj = projectileInstance.GetComponent<EnergyProjectile>();
            ProjectileInfo projectileInstanceInfo = (ProjectileInfo)(proj.itemInfo);
            chargeSphere.SetActive( proj.currentCharge >= projectileInstanceInfo.maxCharge && !proj.isLaunched );
            chargeAura.SetActive( isCharging && isPoweredUp );    
            chargeEffect.SetActive( proj.isCharging );
        }
        else{
            chargeSphere.SetActive( false );
            chargeAura.SetActive( false );
        }
    } 
}
