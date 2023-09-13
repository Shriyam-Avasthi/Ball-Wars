using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/New Projectile")]
public class ProjectileInfo : ItemInfo
{
    public int damagePerCharge;
    public float hitImpulse;
    public float speed;
    public float lifeTime;
    public float maxCharge;
    public float thresholdChargingTime;
    public float chargingTime;
    public float chargedDeltaRadius;
}
