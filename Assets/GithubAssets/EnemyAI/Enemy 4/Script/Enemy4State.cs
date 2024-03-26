using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4State : EnemyCore
{
     public override void AttackState()
    {
         agent.updateRotation = false;
        agent.ResetPath();
        

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack 3"))
        {
            
            if (stateInfo.normalizedTime >= 1.0f)
            {
                state.SwitchStateTo(EnemyStateManager.EnemyState.MoveBackWards);
                return;
            }
        }
        if(stateInfo.normalizedTime <= 0.3)
        {
            canLook = true;
        }
        else
        {
            canLook = false;
        }
        if(canLook)
        {
            LookAtPlayer(0.7f);
        }
        
        if (stateInfo.normalizedTime >= 0.1 && stateInfo.normalizedTime <= 0.5)
        {
            
            SetAttackCollider(true);
            
            
            
        }
        else
        {
            SetAttackCollider(false);
        }
        
        
        if(!canPerformAttack(1.2f) && stateInfo.normalizedTime >= 0.9 )
        {
            
            canLook = true;
            RandomAfterAttack(EnemyStateManager.EnemyState.KickAttack);    
            
        }
    }

    public override void ChasePlayer()
    {
        agent.updateRotation = true;
        
        agent.SetDestination(player.position);
        agent.speed = 3;

        playerLossSwitch(EnemyStateManager.EnemyState.Idle);

        if(canPerformAttack())
        {
            state.SwitchStateTo(EnemyStateManager.EnemyState.Attacking);
            
        }
    }

    public override void DeathState()
    {
        agent.ResetPath();
        GetComponent<Collider>().enabled = false;
        StartCoroutine(AfterDeath());
    }

    public override void FlankingLeft()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        LookAtPlayer();
        // agent.speed = 1f;
        // agent.SetDestination(leftPoint.position);
        flankingTimeCounter += Time.deltaTime;
        if (flankingTimeCounter >= (flankingTime*0.3f))
        {
            if (canPerformAttack())
            {
                if (stateInfo.normalizedTime >= .95f)
                {
                    state.SwitchStateTo(EnemyStateManager.EnemyState.Attacking);
                }
                return;
            }
            if (!GetPlayer())
            {
                if (stateInfo.normalizedTime >= .95f)
                {
                    RandomChooseFar();
                }
                return;
            }
        }
        if (flankingTimeCounter >= flankingTime)
        {
            if (!hasDone)
            {
                if (stateInfo.normalizedTime >= .95f)
                {
                    RandomChoose(EnemyStateManager.EnemyState.FlankRight);
                }
                hasDone = true;
            }
        }
        hasDone = false;
    }

    public override void FlankingRight()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        LookAtPlayer();
        // agent.speed = 1f;
        // agent.SetDestination(rightPoint.position);
        flankingTimeCounter += Time.deltaTime;
        if (flankingTimeCounter >= (flankingTime*0.3f))
        {
            if (canPerformAttack())
            {
                if (stateInfo.normalizedTime >= .95f)
                {
                    state.SwitchStateTo(EnemyStateManager.EnemyState.Attacking);
                }
                return;
            }
            if (!GetPlayer())
            {
               if (stateInfo.normalizedTime >= .95f)
               {
                 RandomChooseFar();
               }
                return;
            }
        }
        if (flankingTimeCounter >= flankingTime)
        {
            if(!hasDone)
            {
                if (stateInfo.normalizedTime >= .95f)
                {
                    RandomChoose(EnemyStateManager.EnemyState.FlankLeft);
                }
                hasDone = true;
            }
        }
        hasDone = false;
    }

    public override void Kicking()
    {
        agent.updateRotation = false;
        agent.ResetPath();
        
        if (canLook)
        {
            LookAtPlayer(1f);
        }
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("KickAttack"))
        {
            if (stateInfo.normalizedTime >= 0.35 && stateInfo.normalizedTime <= 0.56 )
            {
                
                if(stateInfo.normalizedTime <= 0.4)
                {
                    canLook = false;
                    playerNewPos = player.position;
                }
                attackColliderKick.enabled = true;
                
                
            }
            else
            {
                attackColliderKick.enabled = false;
            }
        }

        if(stateInfo.IsName("KickAttack") && stateInfo.normalizedTime >= 1.0f)
        {
            canLook = true;
            state.SwitchStateTo(EnemyStateManager.EnemyState.MoveBackWards);
        }
    }

    public override void MovingBackwards()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        LookAtPlayer(2);
        agent.speed = 1f;
        agent.updateRotation = false;
        agent.SetDestination(backPoint.position);
        flankingTimeCounter += Time.deltaTime;
        if(stateInfo.IsName("MovingBackwards"))
        {
            anim.speed = -1;
        }
        else
        {
            anim.speed = 1;
        }
        if (flankingTimeCounter >= (flankingTime*0.3f))
        {
            if (canPerformAttack())
            {
                state.SwitchStateTo(EnemyStateManager.EnemyState.Attacking);
                return;
            }
        }
        if (flankingTimeCounter >= flankingTime || !GetPlayer())
        {
            RandomAfterAttack(EnemyStateManager.EnemyState.KickAttack);
        }
    }

    public override void WaitForPlayer()
    {
        anim.SetBool("Idle",false);
        if(GetPlayer()){
            state.SwitchStateTo(EnemyStateManager.EnemyState.Chasing);
        }
        else
        {
            EnemyPatrol();
        }
    }
}