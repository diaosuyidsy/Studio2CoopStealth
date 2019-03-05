using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController.vActions;
using UnityEngine;

public class PickObj : MonoBehaviour
{
    public Transform HoldingPos; //holding pos on player
    public Transform[] pickUpTriggers;
    public Transform holdPosOnObj; 
    private bool isFalling = false;
    public IKAnimation ikAnimation;


    public void SetObjParent(Transform triggerPos)
    {        
        transform.parent = HoldingPos;
        transform.position = HoldingPos.position;
        transform.Find("Mesh").GetComponent<Collider>().isTrigger = true;
        foreach (var trigger in pickUpTriggers)
        {
            trigger.GetComponent<Collider>().enabled = false;
        }
        
        GetComponent<Rigidbody>().isKinematic = true;
        
        transform.Find("putdown").GetComponent<vTriggerGenericAction>().enabled = true;
        transform.Find("putdown").GetComponent<Collider>().enabled = true;
        Vector3 dir = Vector3.Scale(transform.position - triggerPos.position,new Vector3(1,0,1)).normalized;
        holdPosOnObj.rotation = Quaternion.LookRotation(dir);
        ikAnimation.setHandPos(holdPosOnObj.Find("RightHandPos"),holdPosOnObj.Find("LeftHandPos"));
    }
    
    public void PutDown()
    {
        transform.parent = null;
        //transform.position = HoldingPos.position;
        StartCoroutine(waitForDrop());
        transform.rotation = Quaternion.Euler(Vector3.Scale(transform.rotation.eulerAngles,new Vector3(0,1,0)));
        GetComponent<Rigidbody>().isKinematic = false;
        //GetComponent<Rigidbody>().centerOfMass = new Vector3(0,-0.5f,0);
        GetComponent<Rigidbody>().velocity += HoldingPos.up*3 + Vector3.down * 10;
        transform.Find("putdown").GetComponent<vTriggerGenericAction>().enabled = false;
        transform.Find("putdown").GetComponent<Collider>().enabled = false;
        foreach (var trigger in pickUpTriggers)
        {
            trigger.GetComponent<Collider>().enabled = true;
        }
        

    }
    

    IEnumerator waitForDrop()
    {
        yield return new WaitForSeconds(0.1f);
        isFalling = true;
        transform.Find("Mesh").GetComponent<Collider>().isTrigger = false;
    }

    private void Update()
    {

        if (isFalling)
        {
            Ray ray  = new Ray(transform.position, Vector3.down);
            float raycastDist = 0.05f;
            Debug.DrawRay(ray.origin, ray.direction * raycastDist, Color.yellow);
            RaycastHit rayHit = new RaycastHit();
            if (Physics.Raycast(ray, out rayHit,raycastDist))
            {
                if (!rayHit.transform.tag.Equals("Player"))
                {
                    GetComponent<Rigidbody>().isKinematic = true;
                    isFalling = false;
                }


            }
        }
    }

    private void FixedUpdate()
    {
        
        /*if (isFalling)
        {
            if(Mathf.Abs(GetComponent<Rigidbody>().velocity.y)<0.003f)
            {
                isFalling = false;
                GetComponent<Rigidbody>().isKinematic = true;
            }
        }*/
    }
}
