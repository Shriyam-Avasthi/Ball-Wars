using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : Item
{
    public abstract void Launch(Vector3 dir);
    public abstract void OnTriggerStay(Collider collider);
}
