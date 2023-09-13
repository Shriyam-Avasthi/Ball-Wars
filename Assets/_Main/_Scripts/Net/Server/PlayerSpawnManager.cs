using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance;
    [SerializeField] SpawnPoint[] spawnPoints;
    
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

}
