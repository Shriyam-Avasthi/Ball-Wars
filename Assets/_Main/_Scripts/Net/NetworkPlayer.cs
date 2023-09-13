using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public bool isMine;
    public int hitterID = 0;
    public Vector3 hitDirection;
    public GameObject[] powerUpObjects;

    [SerializeField] private ParticleSystem dissolveParticleEffect;

    private void Start()
    {
        if( playerInfo.PlayerID == Client.Instance.MyPlayerID )
        {
            isMine = true;
        }
        hitDirection = Vector3.zero;
        
    }

    public void OnLavaContact()
    {
        dissolveParticleEffect.Play();
        StartCoroutine("PlayLavaDissolve");
    }

    private IEnumerator PlayLavaDissolve()
    {
        while(dissolveParticleEffect.IsAlive() )
        {
            yield return null;
        }
        dissolveParticleEffect.Stop();
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if( isMine && NetworkPlayerManager.Instance != null )
        {
            Debug.Log(hitterID);
            if( hitterID != 0 )
            {
                NetworkPlayerManager.Instance.AwardPointsToPlayerWithID( hitterID );
            }
            NetworkPlayerManager.Instance.StartRespawn();
        }
    }

}
