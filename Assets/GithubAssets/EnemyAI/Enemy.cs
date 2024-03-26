
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField] Transform[] patrolPoint;
    private int currentPoint;
    private NavMeshAgent agent;
    [SerializeField] Transform leftPoint,rightPoint,backPoint;
    [SerializeField] float moveSpeed;
    [SerializeField] float aggroDistance;
    [SerializeField] float attackTriggerDistance;
    [SerializeField] float playerLossCountdown = 1;
    [SerializeField] float flankingTime;
    [SerializeField] float rotationSpeed;
    private float flankingTimeCounter;
    private float playerLossCounter;
    Transform player;
    Animator anim;
    Vector3 playerNewPos;
    public bool isAttackFinish;
    bool isChanging;
    public bool isDead;
    public enum EnemyState
    {
        Idle,Chasing,Attacking,FlankLeft,FlankRight,MoveBackWards,KickAttack,Death
    }

    public EnemyState currentState;
  
    bool hasDone;
    bool canLook;
   
    [SerializeField] Collider attackCollider,attackColliderKick;
    [SerializeField] public Transform lockOnTarget;
    // Start is called before the first frame update
    void Start()
    {
        
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        foreach (Transform point in patrolPoint)
        {
            point.SetParent(null);
        }
        currentPoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        Debug.Log(currentState);
        switch(currentState)
        {
            case EnemyState.Idle:
                WaitForPlayer();
                break;
            case EnemyState.Chasing:
                ChasePlayer();
                break;    
            case EnemyState.Attacking:
                AttackState();
                break;
            case EnemyState.FlankLeft:
                FlankingLeft();
                break;
            case EnemyState.FlankRight:
                FlankingRight();
                break;
            case EnemyState.MoveBackWards:
                MovingBackwards();
                break;
            case EnemyState.KickAttack:
                Kicking();
                break;
            case EnemyState.Death:
                DeathState();
                break;
        }

        

    }

    private void WaitForPlayer()
    {
        anim.SetBool("Idle",false);
        if(GetPlayer()){
            SwitchStateTo(EnemyState.Chasing);
        }
        else
        {
            EnemyPatrol();
        }
    }

    private void EnemyPatrol()
    {
        
        if(Mathf.Abs(transform.position.x - patrolPoint[currentPoint].position.x) >= 0.2f)
        {
            anim.SetBool("Walking",true);
            agent.speed = 1;
            agent.SetDestination(patrolPoint[currentPoint].position);
        }
        else
        {
            
            anim.SetBool("Walking",false);
            
            if (!isChanging)
            {
                
                StartCoroutine(ChangePoint());
                isChanging = true;
            }
            
            
        }
        // Debug.Log(Mathf.Abs(transform.position.x - patrolPoint[currentPoint].position.x));
    }
    private void ChasePlayer()
    {
        agent.updateRotation = true;
        // LookAtPlayer();
        agent.SetDestination(player.position);
        agent.speed = 3;

        playerLossSwitch(EnemyState.Idle);

        if(canPerformAttack())
        {
            SwitchStateTo(EnemyState.Attacking);
            
        }

        
        
    }

    private void FlankingLeft()
    {
        LookAtPlayer();
        agent.speed = 1f;
        agent.SetDestination(leftPoint.position);
        flankingTimeCounter += Time.deltaTime;
        if (flankingTimeCounter >= (flankingTime*0.3f))
        {
            if (canPerformAttack())
            {
                SwitchStateTo(EnemyState.Attacking);
                return;
            }
            if (!GetPlayer())
            {
                RandomChooseFar();
                return;
            }
        }
        if (flankingTimeCounter >= flankingTime)
        {
            if (!hasDone)
            {
                RandomChoose(EnemyState.FlankRight);
                hasDone = true;
            }
        }
        hasDone = false;
    }

    private void FlankingRight()
    {
        LookAtPlayer();
        agent.speed = 1f;
        agent.SetDestination(rightPoint.position);
        flankingTimeCounter += Time.deltaTime;
        if (flankingTimeCounter >= (flankingTime*0.3f))
        {
            if (canPerformAttack())
            {
                SwitchStateTo(EnemyState.Attacking);
                return;
            }
            if (!GetPlayer())
            {
                RandomChooseFar();
                return;
            }
        }
        if (flankingTimeCounter >= flankingTime)
        {
            if(!hasDone)
            {
                RandomChoose(EnemyState.FlankLeft);
                hasDone = true;
            }
        }
        hasDone = false;
        
    }
    
    private void Kicking()
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
                transform.position = Vector3.MoveTowards(transform.position, playerNewPos, Time.deltaTime * moveSpeed* 8);
                
            }
            else
            {
                attackColliderKick.enabled = false;
            }
        }

        if(stateInfo.IsName("KickAttack") && stateInfo.normalizedTime >= 1.0f)
        {
            canLook = true;
            SwitchStateTo(EnemyState.MoveBackWards);
        }
    }
    private void AttackState()
    {
        agent.updateRotation = false;
        agent.ResetPath();
        

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack 3"))
        {
            
            if (stateInfo.normalizedTime >= 1.0f)
            {
                SwitchStateTo(EnemyState.MoveBackWards);
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
        
        if (stateInfo.normalizedTime >= 0.2 && stateInfo.normalizedTime <= 0.4)
        {
            
            attackCollider.enabled = true;
            transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * moveSpeed);
            
        }
        else
        {
            attackCollider.enabled = false;
        }
        
        
        if(!canPerformAttack(1.2f) && stateInfo.normalizedTime >= 0.9 )
        {
            
            canLook = true;
            RandomAfterAttack(EnemyState.KickAttack);    
            
        }

    
        
    }
    private void MovingBackwards()
    {
        LookAtPlayer(2);
        agent.speed = 1f;
        agent.updateRotation = false;
        agent.SetDestination(backPoint.position);
        flankingTimeCounter += Time.deltaTime;
        if (flankingTimeCounter >= (flankingTime*0.3f))
        {
            if (canPerformAttack())
            {
                SwitchStateTo(EnemyState.Attacking);
                return;
            }
        }
        if (flankingTimeCounter >= flankingTime || !GetPlayer())
        {
            RandomAfterAttack(EnemyState.KickAttack);
        }
    }
    private void LookAtPlayer(float multiplier = 1)
    {
        Vector3 playerPosition = new Vector3(player.position.x,transform.position.y,player.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(playerPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (rotationSpeed*multiplier) * Time.deltaTime);
        // transform.LookAt(playerPosition);
    }
   
    private void playerLossSwitch(EnemyState lossState)
    {
        if(!GetPlayer())
        {
            
            playerLossCounter += Time.deltaTime;
            if(playerLossCounter >= playerLossCountdown)
            {
                
                SwitchStateTo(lossState);
                playerLossCounter = 0;
            }
        }
    }
    private void SwitchStateTo(EnemyState newState)
    {

        switch(currentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chasing:
                playerLossCounter = 0;
                anim.SetBool("Chasing",false);
                agent.SetDestination(transform.position);
                break;
            case EnemyState.Attacking:
                anim.SetBool("Attack",false);
                break; 
            case EnemyState.FlankLeft:
                anim.SetBool("FlankLeft",false);
                flankingTimeCounter = 0; 
                agent.SetDestination(transform.position);
                break;
            case EnemyState.FlankRight:
                anim.SetBool("FlankRight",false);
                flankingTimeCounter = 0;
                agent.SetDestination(transform.position);
                break;  
            case EnemyState.MoveBackWards:
                anim.SetBool("MoveBack",false);
                agent.updateRotation = true;
                flankingTimeCounter = 0;
                agent.SetDestination(transform.position);
                break; 
            case EnemyState.KickAttack:
                anim.SetBool("KickAttack",false);
                break;
            case EnemyState.Death:
                anim.SetBool("Death",false);
                break;
        }

        
        switch(newState)
        {
            case EnemyState.Idle:
                anim.SetBool("Idle",true);
                Debug.Log("Idle Start");
                break;
            case EnemyState.Chasing:
                anim.SetBool("Chasing",true);
                break;
            case EnemyState.Attacking:
                anim.SetBool("Attack",true);
                break;    
            case EnemyState.FlankLeft:
                anim.SetBool("FlankLeft",true);
                hasDone = false;
                break;
            case EnemyState.FlankRight:
                anim.SetBool("FlankRight",true);
                hasDone = false;
                break;
            case EnemyState.MoveBackWards:
                anim.SetBool("MoveBack",true);
                break;
            case EnemyState.KickAttack:
                anim.SetBool("KickAttack",true);
                break;
            case EnemyState.Death:
                isDead = true;
                anim.SetBool("Death",true);
                break;
        }

        currentState = newState;
    }

    private void DeathState()
    {
        agent.ResetPath();
        GetComponent<Collider>().enabled = false;
        StartCoroutine(AfterDeath());

    }
    private bool GetPlayer()
    {
        return Vector3.Distance(player.position, transform.position) <= aggroDistance;
    }

    private bool canPerformAttack(float multiplier = 1)
    {
        return Vector3.Distance(player.position, transform.position) <= attackTriggerDistance * multiplier;
    }

    private IEnumerator ChangePoint()
    {
        
        yield return new WaitForSeconds(1);
        currentPoint++;
        if (currentPoint >= patrolPoint.Length)
        {
            currentPoint = 0;
        }
       
        isChanging = false;

    }
    
    private void RandomChooseFar()
    {
        
        int rand = Random.Range(1,3);
        
        switch (rand)
        {
            case 1 :
                
                SwitchStateTo(EnemyState.KickAttack);
                
                break;
            case 2 :
                SwitchStateTo(EnemyState.Chasing);
                break;
            
            
        }
        
    }
    private void RandomChoose(EnemyState flank)
    {
        
        int rand = Random.Range(1,4);
        
        switch (rand)
        {
            case 1 :
                if(GetPlayer())
                {
                    SwitchStateTo(EnemyState.Attacking);
                } else
                {
                    RandomChooseFar();
                }
                
                break;
            case 2 :
                SwitchStateTo(flank);
                break;
            case 3 :
                SwitchStateTo(EnemyState.KickAttack);
                break;
            
        }
        
    }
    private void RandomAfterAttack(EnemyState whatAttack)
    {
        
        int rand = Random.Range(1,5);
        
        switch (rand)
        {
            case 1 :
                
                SwitchStateTo(whatAttack);
                break;
            case 2 :
                SwitchStateTo(EnemyState.FlankLeft);
                break;
            case 3 :
                SwitchStateTo(EnemyState.FlankRight);
                break;
            case 4 :
                SwitchStateTo(EnemyState.Chasing);
                break;
            
        }
        
    }
     IEnumerator AfterDeath()
    {
        yield return new WaitForSeconds(5);
        this.gameObject.SetActive(false);
    }
    public void GetDamage()
    {
        if(currentState != EnemyState.Attacking)
        {
            anim.SetTrigger("Hit");
        }
        
    }
    public void EnemyDeath()
    {
            SwitchStateTo(EnemyState.Death);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, aggroDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackTriggerDistance);

    }

   

}
