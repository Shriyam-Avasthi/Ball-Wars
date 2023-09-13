using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime;
    private float timeElapsed;
    private void Update()
    {
        if(timeElapsed >= lifetime)
        {
            Destroy(this.gameObject);
        }
        timeElapsed += Time.deltaTime;
    }
}
