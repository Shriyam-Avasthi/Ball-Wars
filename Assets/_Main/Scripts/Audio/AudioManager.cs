using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private GameObject oneShotAudioPrefab;

    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip inGameMusic;
    [SerializeField] private AudioClip buttonClickSfx;

    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private AudioClip[] energyProjectileExplosionSfxs ;
    [SerializeField] private AudioClip[] charged_energyProjectileExplosionSfxs ;

    [SerializeField] private AudioClip weaponPowerUpSpawnSfx ;
    [SerializeField] private AudioClip weaponPowerUpCollectSfx ;

    [SerializeField] private float backgroundMusicTransitionTime = 1.0f;    

    private void Awake()
    {
        if( Instance != null)
        {
            Destroy( Instance.gameObject );
        }
        Instance = this;
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    public void PlayButtonClickSfx()
    {
        sfxAudioSource.PlayOneShot( buttonClickSfx );
    }

    public void FadeNewBackgroundMusic( AudioClip clip , float transitionTime = 1.0f )
    {
        StartCoroutine( FadeMusic(backgroundAudioSource , clip , transitionTime) );
    }

    private IEnumerator FadeMusic( AudioSource activeSource , AudioClip newClip , float transitionTime )
    {   
        float t;
        if( activeSource.clip != null )
        {    
            t = 0;
            while( t < transitionTime  / 2 )
            {
                t += Time.deltaTime;
                activeSource.volume = 1 - ( (2 * t) / transitionTime) ;
                yield return null;
            }
            activeSource.Stop();
        }

        activeSource.clip = newClip;
        activeSource.Play();
        t = 0;
        while( t < transitionTime / 2 )
        {
            t += Time.deltaTime;
            activeSource.volume = ( (2 * t) / transitionTime) ;
            yield return null;
        }

    }

    public void PlayMenuMusic()
    {
        FadeNewBackgroundMusic( menuMusic , backgroundMusicTransitionTime );
    }

    public void PlayInGameMusic()
    {
        FadeNewBackgroundMusic( inGameMusic , backgroundMusicTransitionTime );
    }

    public void PlayJumpSfx( Vector3 position )
    {
        AudioSource source = Instantiate( oneShotAudioPrefab , position , Quaternion.identity).GetComponent<AudioSource>();
        source.clip = jumpSfx;
        source.Play();
        Destroy( source.gameObject , source.clip.length);
    }

    public void PlayEnergyProjectileExplosionSfx( Vector3 position , float chargeRatio, bool isPoweredUp )
    {
        if( chargeRatio > 1f )
        {
            chargeRatio = 1f;
        }
        if( isPoweredUp)
        {
            int index = (int)Mathf.Floor(chargeRatio * (charged_energyProjectileExplosionSfxs.Length - 1) ) ;

            AudioSource source = Instantiate( oneShotAudioPrefab , position , Quaternion.identity).GetComponent<AudioSource>();
            source.clip = charged_energyProjectileExplosionSfxs[index];
            source.Play();
            Destroy( source.gameObject , source.clip.length);
        }
        else
        {
            int index = (int)Mathf.Floor(chargeRatio * (energyProjectileExplosionSfxs.Length - 1) ) ;

            AudioSource source = Instantiate( oneShotAudioPrefab , position , Quaternion.identity).GetComponent<AudioSource>();
            source.clip = energyProjectileExplosionSfxs[index];
            source.Play();
            Destroy( source.gameObject , source.clip.length);
        }
    }

    public void PlayPowerUpSpawnSfx( Vector3 position , PowerUpType type)
    {
        AudioSource source = Instantiate( oneShotAudioPrefab , position , Quaternion.identity).GetComponent<AudioSource>();
        if( type == PowerUpType.WeaponPowerUp)
        {
            source.clip = weaponPowerUpSpawnSfx;
        }
        source.Play();
        Destroy(source.gameObject , source.clip.length);
    }

    public void PlayPowerUpCollectSfx( Vector3 position , PowerUpType type)
    {
        AudioSource source = Instantiate( oneShotAudioPrefab , position , Quaternion.identity).GetComponent<AudioSource>();
        if( type == PowerUpType.WeaponPowerUp)
        {
            source.clip = weaponPowerUpCollectSfx;
        }
        source.Play();
        Destroy(source.gameObject , source.clip.length);
    }

}
