using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TargetLock : MonoBehaviour
{
    [SerializeField]float targetingRadius = 10;
    bool targetLock;
    public bool isLockedOn;
    public Transform currentTarget;
    [SerializeField] Transform cameraObject;
    [SerializeField] float minimumAngle = -50;
    [SerializeField] float maximumAngle = 50;
    [SerializeField] Transform lockOnTarget;
    [SerializeField] LayerMask environtmentLayers,enemyLayers;
    private List<Transform> availableTargets = new List<Transform>();
    Transform nearestTarget;
    Vector3 test;
    [SerializeField] Transform enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        targetLock = Input.GetButtonDown("Fire2");
        Targeting();
    }

    void Targeting()
    {
        if(isLockedOn)
        {
            if(currentTarget == null)
            {
                return;
            }
            if(currentTarget.GetComponent<EnemyCore>().isDead)
            {
                currentTarget = null;
                isLockedOn = false;
                ClearLockOnTargets();
            }
        }
        if(targetLock && isLockedOn)
        {
            Debug.Log("Target, Not");
            ClearLockOnTargets();
            currentTarget = null;
            isLockedOn = false;
            return;
        }
        if(targetLock && !isLockedOn)
        {
            Debug.Log("Target, Locate");
            LocatingTarget();
            if(nearestTarget != null)
            {
                SetTarget(nearestTarget);
                isLockedOn = true;
            }
        }
    }
    void LocatingTarget()
    {
        float shortDistance = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(transform.position,targetingRadius,enemyLayers);

        for (int i =0; i < colliders.Length; i++)
        {
            Transform target = colliders[i].GetComponent<Transform>();
            if (target != null)
            {
                Vector3 targetDirection = target.position - transform.position;
                test = targetDirection;
                float targetDistance = Vector3.Distance(transform.position, target.position);
                float viewAngle = Vector3.Angle(targetDirection,cameraObject.forward);

                if(target.GetComponent<EnemyCore>() == null)
                {
                    continue;
                }
                if(target.GetComponent<EnemyCore>().isDead)
                {
                    continue;
                }
                if(target.root == transform.root)
                {
                    continue;
                }
                Debug.Log("Rights" + viewAngle);
                
                if(viewAngle > minimumAngle && viewAngle < maximumAngle)
                {
                    RaycastHit hit;
                    Debug.Log("Rightshere");
                    Debug.DrawLine(lockOnTarget.position, target.GetComponent<EnemyCore>().lockOnTarget.position, Color.green);
                    if (Physics.Linecast(lockOnTarget.position,target.GetComponent<EnemyCore>().lockOnTarget.position,out hit,environtmentLayers))
                    {
                        Debug.Log("Hidden");
                        
                        continue;
                    }
                    else
                    {
                       
                        availableTargets.Add(target);
                    }
                }
            }
        }

        for (int i = 0; i < availableTargets.Count; i++)
        {
            if(availableTargets[i] != null)
            {
                float distanceFromTarget = Vector3.Distance(transform.position, availableTargets[i].transform.position);
               
                if (distanceFromTarget < shortDistance)
                {
                    shortDistance = distanceFromTarget;
                    nearestTarget = availableTargets[i];
                }
            }
            else
            {
                ClearLockOnTargets();
                isLockedOn = false;
            }
        }
    }

    private void ClearLockOnTargets()
    {
        availableTargets.Clear();
        nearestTarget = null;
        
    }

    private void SetTarget(Transform newTarget)
    {
        if(newTarget != null)
        {
            currentTarget = newTarget;
        }
    }

    private void LockOnChange(bool old, bool isLockedOn)
    {
        
        if(!isLockedOn)
        {
            currentTarget = null;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(cameraObject.position, cameraObject.forward * 10f);

        // Visualisasikan vektor arah target menggunakan gizmo
        Gizmos.color = Color.green;
        Gizmos.DrawRay(cameraObject.position, test.normalized * 10f);
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,targetingRadius);
    }
}
