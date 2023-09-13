using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour , IDamageable
{ 
    [field : SerializeField]
    public int maxHP{get; private set;}
    public int HP{get; set;}

    private void Awake()
    {
        HP = maxHP;
    }

    public virtual void ApplyDamage(int damage)
    {
        HP -= damage;
        if( HP <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}  
