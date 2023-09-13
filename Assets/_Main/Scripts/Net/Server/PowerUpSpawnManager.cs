using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerUpSpawnManager : MonoBehaviour
{
    public static PowerUpSpawnManager Instance;
    [SerializeField] private SpawnPoint[] spawnPoints;
    [SerializeField] private PowerUp[] powerUps;
    
    private System.Random random;

    private void Awake()
    {
        if( Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        random = new System.Random();
    }

    public Vector3 GetRandomSpawnPosition()
    {
        int randomIndex = random.Next( spawnPoints.Length );
        return spawnPoints[randomIndex].gameObject.transform.position;
    }

    public int GetRandomPowerUp()
    {
        int randomIndex = random.Next( powerUps.Length );
        return ((PowerUpInfo)powerUps[randomIndex].itemInfo).ID;
    }

}
