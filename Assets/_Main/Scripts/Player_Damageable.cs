using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Damageable : Damageable
{
    public override void ApplyDamage( int damage )
    {
        HP -= damage;
        if( HP <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
