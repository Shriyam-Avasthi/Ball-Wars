using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int maxHP{get;}
    int HP{get; set; }
    void ApplyDamage(int damage);
}
