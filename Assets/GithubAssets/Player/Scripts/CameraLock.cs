using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraLock : MonoBehaviour
{
    [SerializeField]CinemachineVirtualCamera lockCamera;
    [SerializeField]CinemachineFreeLook freeCamera;
    [SerializeField]TargetLock lockOn;
    [SerializeField]RectTransform lockUi;
    
   Vector3 scrPos;
    float counter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(lockOn.isLockedOn)
       {    counter += Time.deltaTime;
            
            lockCamera.Priority = 99;
            lockCamera.LookAt = lockOn.currentTarget.Find("TargetLock");
            scrPos = Camera.main.WorldToScreenPoint(lockOn.currentTarget.Find("LookAtPlayer").position);
            
            
            lockUi.position = scrPos;
            lockUi.gameObject.SetActive(true);
            Debug.Log(lockCamera.transform.eulerAngles.y);
            if(counter >.5)
            {
                counter = 5;
                freeCamera.m_XAxis.Value = lockCamera.transform.eulerAngles.y;
                freeCamera.m_YAxis.Value = .5f;
            }
            
            // freeCamera.m_YAxis.Value = .5f;
            
       }
       else
       {
            lockUi.gameObject.SetActive(false);
            counter = 0;
            lockCamera.Priority = 0;
            lockCamera.LookAt = null;
            
       }
       

       
    }

   
   

   
}
