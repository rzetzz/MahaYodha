using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,Chasing,Attacking,FlankLeft,FlankRight,MoveBackWards,KickAttack,Death
    }

    public EnemyState currentState;

    [SerializeField]EnemyCore state;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         switch(currentState)
        {
            case EnemyState.Idle:
                state.WaitForPlayer();
                break;
            case EnemyState.Chasing:
                state.ChasePlayer();
                break;    
            case EnemyState.Attacking:
                state.AttackState();
                break;
            case EnemyState.FlankLeft:
                state.FlankingLeft();
                break;
            case EnemyState.FlankRight:
                state.FlankingRight();
                break;
            case EnemyState.MoveBackWards:
                state.MovingBackwards();
                break;
            case EnemyState.KickAttack:
                state.Kicking();
                break;
            case EnemyState.Death:
                state.DeathState();
                break;
        }
    }

    public void SwitchStateTo(EnemyState newState)
    {

        switch(currentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chasing:
                state.playerLossCounter = 0;
                state.anim.SetBool("Chasing",false);
                state.agent.SetDestination(transform.position);
                break;
            case EnemyState.Attacking:
                state.anim.SetBool("Attack",false);
                break; 
            case EnemyState.FlankLeft:
                state.anim.SetBool("FlankLeft",false);
                state.flankingTimeCounter = 0; 
                state.agent.SetDestination(transform.position);
                break;
            case EnemyState.FlankRight:
                state.anim.SetBool("FlankRight",false);
                state.flankingTimeCounter = 0;
                state.agent.SetDestination(transform.position);
                break;  
            case EnemyState.MoveBackWards:
                state.anim.SetBool("MoveBack",false);
                state.agent.updateRotation = true;
                state.flankingTimeCounter = 0;
                state.agent.SetDestination(transform.position);
                break; 
            case EnemyState.KickAttack:
                state.anim.SetBool("KickAttack",false);
                break;
            case EnemyState.Death:
                state.anim.SetBool("Death",false);
                break;
        }

        
        switch(newState)
        {
            case EnemyState.Idle:
                state.anim.SetBool("Idle",true);
                Debug.Log("Idle Start");
                break;
            case EnemyState.Chasing:
                state.anim.SetBool("Chasing",true);
                break;
            case EnemyState.Attacking:
                state.anim.SetBool("Attack",true);
                break;    
            case EnemyState.FlankLeft:
                state.anim.SetBool("FlankLeft",true);
                state.hasDone = false;
                break;
            case EnemyState.FlankRight:
                state.anim.SetBool("FlankRight",true);
                state.hasDone = false;
                break;
            case EnemyState.MoveBackWards:
                state.anim.SetBool("MoveBack",true);
                break;
            case EnemyState.KickAttack:
                state.anim.SetBool("KickAttack",true);
                break;
            case EnemyState.Death:
                state.isDead = true;
                state.anim.SetBool("Death",true);
                break;
        }

        currentState = newState;
    }
}
