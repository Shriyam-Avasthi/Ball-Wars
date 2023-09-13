using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
    public Transform weaponPivot;
    public abstract void Use();
    public abstract void Aim();
}
