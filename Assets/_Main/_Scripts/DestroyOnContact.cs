using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    [SerializeField] private LayerMask ignoredLayer;
    private void OnCollisionEnter(Collision collision)
    {
        if( collision.collider.gameObject.GetComponent<IDamageable>() != null  && collision.collider.gameObject.layer != ignoredLayer)
        {
            if( collision.collider.gameObject.CompareTag("Player"))
            {
                collision.collider.gameObject.GetComponent<NetworkPlayer>().OnLavaContact();
            }
            else
            {
                Destroy(collision.collider.gameObject);
            }
        }
    }
}
