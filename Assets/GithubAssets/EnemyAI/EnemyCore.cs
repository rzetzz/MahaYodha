
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyCore : MonoBehaviour
{

    [SerializeField] Transform[] patrolPoint;
    private int currentPoint;
    public NavMeshAgent agent {get; protected set;}
    [SerializeField] protected Transform leftPoint,rightPoint,backPoint;
    public float moveSpeed;
    public float walkSpeed = 1;
    public float runSpeed = 3;
    [SerializeField] float aggroDistance;
    [SerializeField] float attackTriggerDistance;
    [SerializeField] float playerLossCountdown = 1;
    [SerializeField] public float flankingTime;
    [SerializeField] float rotationSpeed;
    public float flankingTimeCounter {get; set;}
    public float playerLossCounter {get; set;}
    protected Transform player;
    public Animator anim {get ; protected set; }
    public Vector3 playerNewPos;
    public bool isAttackFinish;
    bool isChanging;
    public bool isDead;
    
  
    public bool hasDone {get; set;}
    public bool canLook;
    protected EnemyStateManager state;

    [SerializeField] public Collider[] attackCollider;
    public Collider attackColliderKick;
    [SerializeField] public Transform lockOnTarget;
    // Start is called before the first frame update
    void Start()
    {
        state = GetComponent<EnemyStateManager>();
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
          

    }

    protected void SetAttackCollider(bool what)
    {
        foreach (Collider item in attackCollider)
        {
            item.enabled = what;
        }
    }
    public abstract void WaitForPlayer();
    
    protected void EnemyPatrol()
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
    public abstract void ChasePlayer();
    

    public abstract void FlankingLeft();
    

    public abstract void FlankingRight();
    
    
    public abstract void Kicking();
    
    public abstract void AttackState();

    public abstract void MovingBackwards();
    
    protected void LookAtPlayer(float multiplier = 1)
    {
       if (Vector3.Distance(transform.position,player.transform.position) > 1)
       {
         Vector3 playerPosition = new Vector3(player.position.x,transform.position.y,player.position.z);
         Quaternion targetRotation = Quaternion.LookRotation(playerPosition - transform.position);
         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (rotationSpeed*multiplier) * Time.deltaTime);
       }
        // transform.LookAt(playerPosition);
    }
   
    protected void playerLossSwitch(EnemyStateManager.EnemyState lossState)
    {
        if(!GetPlayer())
        {
            
            playerLossCounter += Time.deltaTime;
            if(playerLossCounter >= playerLossCountdown)
            {
                
                state.SwitchStateTo(lossState);
                playerLossCounter = 0;
            }
        }
    }


    public abstract void DeathState();
    
    protected bool GetPlayer()
    {
        return Vector3.Distance(player.position, transform.position) <= aggroDistance;
    }

    protected bool canPerformAttack(float multiplier = 1)
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
    
    protected void RandomChooseFar()
    {
        
        int rand = Random.Range(1,3);
        
        switch (rand)
        {
            case 1 :
                
                state.SwitchStateTo(EnemyStateManager.EnemyState.KickAttack);
                
                break;
            case 2 :
                state.SwitchStateTo(EnemyStateManager.EnemyState.Chasing);
                break;
            
            
        }
        
    }
    protected void RandomChoose(EnemyStateManager.EnemyState flank)
    {
        
        int rand = Random.Range(1,4);
        
        switch (rand)
        {
            case 1 :
                if(GetPlayer())
                {
                    state.SwitchStateTo(EnemyStateManager.EnemyState.Attacking);
                } else
                {
                    RandomChooseFar();
                }
                
                break;
            case 2 :
                state.SwitchStateTo(flank);
                break;
            case 3 :
                state.SwitchStateTo(EnemyStateManager.EnemyState.KickAttack);
                break;
            
        }
        
    }
    protected void RandomAfterAttack(EnemyStateManager.EnemyState whatAttack)
    {
        
        int rand = Random.Range(1,5);
        
        switch (rand)
        {
            case 1 :
                
                state.SwitchStateTo(whatAttack);
                break;
            case 2 :
                state.SwitchStateTo(EnemyStateManager.EnemyState.FlankLeft);
                break;
            case 3 :
                state.SwitchStateTo(EnemyStateManager.EnemyState.FlankRight);
                break;
            case 4 :
                state.SwitchStateTo(EnemyStateManager.EnemyState.Chasing);
                break;
            
        }
        
    }
    public IEnumerator AfterDeath()
    {
        transform.Find("Body").gameObject.SetActive(false);
        yield return new WaitForSeconds(5);
        this.gameObject.SetActive(false);
    }
    public void GetDamage()
    {
        if(state.currentState != EnemyStateManager.EnemyState.Attacking)
        {
            
            anim.SetTrigger("Hit");
        }
        
    }
    public void EnemyDeath()
    {
            state.SwitchStateTo(EnemyStateManager.EnemyState.Death);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, aggroDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackTriggerDistance);

    }

   

}
