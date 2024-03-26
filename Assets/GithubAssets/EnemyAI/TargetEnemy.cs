using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TargetEnemy : MonoBehaviour
{
    
    Transform player;
    [SerializeField]Transform back;
    [SerializeField]Transform stock;
    Transform defaultPos;
    Vector3 temp;
    // Start is called before the first frame update
    void Start()
    {
        defaultPos = this.transform;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Vector3.Distance(transform.position,player.position) < 2)
        {
           
            transform.position = new Vector3(temp.x,transform.position.y,temp.z);
        } else
        {
            temp = transform.position;
            transform.position = Vector3.MoveTowards(transform.position,new Vector3(stock.position.x,transform.position.y,stock.position.z),10 * Time.deltaTime);
        }

        
    }
}
