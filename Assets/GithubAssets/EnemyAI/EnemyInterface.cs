using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface EnemyInterface
{
    
    public abstract void WaitForPlayer();
    public abstract void ChasePlayer();
    public abstract void AttackState();
    public abstract void FlankingLeft();
    public abstract void FlankingRight();
    public abstract void MovingBackwards();
    public abstract void Kicking();
    public abstract void DeathState();
}
