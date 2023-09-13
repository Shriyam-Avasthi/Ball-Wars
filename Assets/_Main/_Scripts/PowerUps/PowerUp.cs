using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    WeaponPowerUp = 0
}

public abstract class PowerUp : Item
{
    public int InstanceID;
    public PowerUpType Type;
    public abstract void OnTriggerEnter(Collider collider);
    public abstract void SetUp(int powerUpID);
}
