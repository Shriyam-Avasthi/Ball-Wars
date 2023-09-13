using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPowerUpable
{
    PowerUpType PowerUpTriggerType{get;}
    void PowerUp(PowerUpInfo powerUpInfo);
}
