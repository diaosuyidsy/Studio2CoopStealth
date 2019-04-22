using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController.vActions;
using UnityEngine;

public class PushObj : MonoBehaviour
{
    private Transform playerPos;
    private bool isPushing = false;
    private Rigidbody pushRigidbody;
    private Vector3 playerDis;

    private Transform target;
    public bool isFall = false;
    private void Start()
    {
        playerPos = GameObject.Find("Player_Big").transform;
        pushRigidbody = GetComponent<Rigidbody>();
    }

    public void SetTarget(Transform tar)
    {
        target = tar;
        Vector3 playerDir = transform.position - target.position;
        playerDis = new Vector3(playerDir.x , 0, playerDir.z);
    }
    public void MoveObj()
    {
        StartCoroutine(SetPushing());
        /*StartCoroutine(SetParent());
        transform.GetComponent<Rigidbody>().isKinematic = false;*/
    }

    /*IEnumerator SetParent()
    {
        yield return new WaitForSeconds(1f);
        transform.parent = playerPos;
    }*/
    IEnumerator SetPushing()
    {
        yield return new WaitForSeconds(0.3f);
        isPushing = true;
    }

    private void Update()
    {
        if (isPushing)
        {
            pushRigidbody.MovePosition(Vector3.Scale(playerPos.position, new Vector3(1,0,1)) + playerDis);
            Ray forwardRay  = new Ray(transform.position + new Vector3(0,0.3f,0) , playerDis.normalized);
            float forwardRaycastDist = GetComponent<Collider>().bounds.extents.x + 0.2f;
            Debug.DrawRay(forwardRay.origin, forwardRay.direction * forwardRaycastDist, Color.green);
            RaycastHit forwardRayHit = new RaycastHit();
            if (Physics.Raycast(forwardRay, out forwardRayHit,forwardRaycastDist))
            {
                if (!forwardRayHit.transform.tag.Contains("Player"))
                {
                    isPushing = false;
                }


            }
        }
        
            
        Ray ray  = new Ray(transform.position - new Vector3(0,-0.1f,0) - playerDis.normalized * 0.6f, Vector3.down);
        float raycastDist = 0.3f;
        Debug.DrawRay(ray.origin, ray.direction * raycastDist, Color.yellow);
        RaycastHit rayHit = new RaycastHit();
        if (Physics.Raycast(ray, out rayHit,raycastDist))
        {
            if (!rayHit.transform.tag.Contains("Player"))
            {
                isFall = false;
            }


        }
        else
        {
            isFall = true;
        }
        
    }

    public void StopMove()
    {
        isPushing = false;
        StopAllCoroutines();
        //transform.parent = null;
    }

}
