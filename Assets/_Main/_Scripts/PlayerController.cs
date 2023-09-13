using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float deltaVelocityPerSec = 1f;
    [SerializeField] private float lerpMultiplier = 2;         
    [SerializeField] private float directionChangeDeltaVelocityPerSec = 10f;
    [SerializeField] private float jumpHeight;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float minSyncWait;
    
    public bool isGrounded;
    
    private NetworkPlayer netPlayer;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    private int jumpsRemaining = 3;
    private bool isMine;
    private float prev_horizontalInput;
    private float lastSyncTimeElapsed;
    private Vector3 lastSyncedPosition;

    private void OnDisable() {
        if( isMine)
        {    
            playerInputActions.Player.Jump.performed -= Jump;
            playerInputActions.Player.Disable();
        }
    }

    private void OnEnable()
    {
        if( isMine)
        {
            playerInputActions.Player.Jump.performed += Jump;
            playerInputActions.Player.Enable();
        }
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
        netPlayer = this.gameObject.GetComponent<NetworkPlayer>(); 
        if( netPlayer.isMine)
        {
            CinemachineVirtualCamera[] cam = FindObjectsOfType<CinemachineVirtualCamera>();
            cam[0].Follow = this.gameObject.transform;
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.Jump.performed += Jump;
            playerInputActions.Player.Enable();
            isMine = true;
        }
        else
        {
            RegisterEvents();
            isMine = false;
            lastSyncedPosition = this.transform.position;
        }
        
    }

    private void FixedUpdate()
    {
        if ( isMine)
        {
            SyncPosition();
        }
        Vector3 sphereCheckPos = new Vector3( this.transform.position.x , this.transform.position.y - this.transform.localScale.y , this.transform.position.z);
        isGrounded = Physics.CheckSphere( sphereCheckPos , 0.05f , layerMask);
        
        if( isGrounded && rb.velocity.x * netPlayer.hitDirection.x < 0 && isMine )
        {
            // Debug.Log(rb.velocity.x);
            // Debug.Log("Unset");
            netPlayer.hitterID = 0;
            netPlayer.hitDirection = Vector3.zero;
        }
    
        if (isGrounded)
        {
            jumpsRemaining = 2;         // For triple Jump
        }
        Move();
    }

    private void LateUpdate()
    {
        if(!isMine)
        this.transform.position = Vector3.Lerp( this.transform.position , lastSyncedPosition, Time.deltaTime );
    }

    private void SyncPosition()
    {
        if( lastSyncTimeElapsed >= minSyncWait )
        {
            Client.Instance.SendToServer( new NetSyncPosition( Client.Instance.MyPlayerID , this.transform.position ) );
            lastSyncTimeElapsed = 0f;
        }
        lastSyncTimeElapsed += Time.deltaTime;
    }

    private void Move()
    {
        float moveHorizontal;
        if ( isMine )
        {
            moveHorizontal = playerInputActions.Player.Movement.ReadValue<float>();
            if( prev_horizontalInput != moveHorizontal )
            {
                Client.Instance.SendToServer( new NetHorizontalInput( Client.Instance.MyPlayerID , moveHorizontal));
                prev_horizontalInput = moveHorizontal;
            }
        }
        else
        {
            moveHorizontal = prev_horizontalInput;
        }
        
        // if ( (Mathf.Round( Mathf.Abs(rb.velocity.x) * 10) / 10 ) < 0.2f && moveHorizontal == 0 )
        // {
        //     rb.velocity = new Vector3( 0f , rb.velocity.y , 0f );
        // }

        int direction;
        if( moveHorizontal * rb.velocity.x > 0 ) direction = 1;
        else if( moveHorizontal * rb.velocity.x < 0 ) direction = -1;
        else direction = 0;

        if( direction == 1 )
        {
            if( Mathf.Abs(rb.velocity.x) < maxSpeed )
            {
                rb.AddForce( moveHorizontal * new Vector3( deltaVelocityPerSec , 0 , 0 ) * Time.deltaTime , ForceMode.VelocityChange);
            }

            else if( Mathf.Abs(rb.velocity.x) > maxSpeed )
            {
                rb.AddForce( moveHorizontal * new Vector3( -deltaVelocityPerSec , 0 , 0 ) * Time.deltaTime , ForceMode.VelocityChange);
            }
            
            if ( (Mathf.Round( Mathf.Abs(rb.velocity.x) * 10) / 10 ) < (maxSpeed + 0.2)  )
            {
                rb.velocity = Vector3.Lerp(rb.velocity , new Vector3( 0f , rb.velocity.y , 0f) , lerpMultiplier * Time.deltaTime) ;
            }
        }

        else if( direction == -1 )
        {
            if( netPlayer.hitDirection.x != 0)
            {
                if( netPlayer.hitDirection.x * moveHorizontal <= 0 )
                {
                    rb.AddForce( -Mathf.Sign(netPlayer.hitDirection.x) * Mathf.Abs(moveHorizontal) * new Vector3(deltaVelocityPerSec , 0 , 0) * Time.deltaTime , ForceMode.VelocityChange);
                }
            }
            else
            {
                rb.AddForce( moveHorizontal * new Vector3(directionChangeDeltaVelocityPerSec , 0 , 0) * Time.deltaTime , ForceMode.VelocityChange);
            }
        }

        else
        {
            if(rb.velocity.x == 0)
            {
                rb.AddForce( moveHorizontal * new Vector3(directionChangeDeltaVelocityPerSec , 0 , 0) * Time.deltaTime , ForceMode.VelocityChange);
            }

            else
            {
                if( Mathf.Abs(  rb.velocity.x ) > 1 )
                rb.AddForce( -Mathf.Sign(rb.velocity.x) * new Vector3(directionChangeDeltaVelocityPerSec , 0 , 0) * Time.deltaTime , ForceMode.VelocityChange);

                else
                {
                    rb.AddForce( -Mathf.Sign(rb.velocity.x) * new Vector3(350f , 0 , 0) * Time.deltaTime);
                }
            }
            
            
            
        }
        

        // if( moveHorizontal != 0)
        // {
        //     if (Mathf.Abs(rb.velocity.x) < maxSpeed)
        //     {
        //         rb.AddForce( moveHorizontal * new Vector3(deltaVelocityPerSec , 0 , 0) * Time.deltaTime , ForceMode.VelocityChange);
        //     }
        //     else if ( moveHorizontal * rb.velocity.x < 0 )
        //     {
        //         rb.AddForce( moveHorizontal * new Vector3(deltaVelocityPerSec , 0 , 0) * Time.deltaTime , ForceMode.VelocityChange);   
        //     }
        // }
        // else
        // {
        //     if ( Mathf.Abs((int)rb.velocity.x) <= maxSpeed)
        //     {
        //         rb.AddForce( -Mathf.Sign(rb.velocity.x) * new Vector3( deltaVelocityPerSec * Mathf.Abs(rb.velocity.x) , 0 , 0) * Time.deltaTime , ForceMode.VelocityChange );
        //     }
        // }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0 )
        {
            Client.Instance.SendToServer( new NetJumpInput( Client.Instance.MyPlayerID ) );
            float u = Mathf.Sqrt(2 * 9.8f * jumpHeight);
            rb.velocity = new Vector3(rb.velocity.x , u , rb.velocity.z);
            jumpsRemaining -= 1;
            AudioManager.Instance.PlayJumpSfx( this.transform.position );
        }
    }

    private void RegisterEvents()
    {
        NetUtility.C_HORIZONTAL_INPUT+= OnReceivedHorizontalInput;
        NetUtility.C_JUMP_INPUT += OnReceivedJumpInput;
        NetUtility.C_SYNC_POSITION += OnReceivedSyncPosition;
    }

    private void UnregisterEvents()
    {
        NetUtility.C_HORIZONTAL_INPUT -= OnReceivedHorizontalInput;
        NetUtility.C_JUMP_INPUT -= OnReceivedJumpInput;
        NetUtility.C_SYNC_POSITION -= OnReceivedSyncPosition;
    }

    private void OnReceivedHorizontalInput( NetMessage msg )
    {
        NetHorizontalInput horizontalInputMsg = msg as NetHorizontalInput;
        if( horizontalInputMsg.playerID == GetComponent<NetworkPlayer>().playerInfo.PlayerID )
        {
            prev_horizontalInput = horizontalInputMsg.horizontalInput;
        }
    }

    private void OnReceivedJumpInput( NetMessage msg )
    {
        NetJumpInput jumpInputMsg = msg as NetJumpInput;
        if( jumpInputMsg.playerID == GetComponent<NetworkPlayer>().playerInfo.PlayerID )
        {
            float u = Mathf.Sqrt(2 * 9.8f * jumpHeight);
            rb.velocity = new Vector3(rb.velocity.x , u , rb.velocity.z);
            jumpsRemaining -= 1;
        }
    }

    private void OnReceivedSyncPosition( NetMessage msg )
    {
        NetSyncPosition syncPositionMsg = msg as NetSyncPosition;
        if ( syncPositionMsg.playerID == GetComponent<NetworkPlayer>().playerInfo.PlayerID)
        {
            lastSyncedPosition = syncPositionMsg.position;
        }
    }

}
