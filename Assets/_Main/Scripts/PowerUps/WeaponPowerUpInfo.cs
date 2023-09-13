using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/PowerUp/New Weapon PowerUp")]
public class WeaponPowerUpInfo : PowerUpInfo
{
    public int deltaDamagePerCharge;
    public float deltaMaxSize;
    public float deltaMaxCharge;
    public float deltaChargingTime;
}
