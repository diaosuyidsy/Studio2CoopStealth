using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController.vActions;
using cakeslice;
using UnityEngine;

public class PickObj : MonoBehaviour
{
    public List<GameObject> Meshes;
    private Transform HoldingPos; //holding pos on player
    public Transform[] pickUpTriggers;
    public Transform holdPosOnObj; 
    private bool isFalling = false;
    private IKAnimation ikAnimation;
    private float fallTime = 0; 
    private void Start()
    {
        ikAnimation = GameObject.Find("Player_Big").GetComponent<IKAnimation>();
        HoldingPos = GameObject.Find("Player_Big").transform.Find("hip").Find("HoldingPos");
    }


    public void SetObjParent(Transform triggerPos)
    {   
        transform.position = HoldingPos.position;
        transform.parent = HoldingPos;
        
        
        transform.GetComponent<Collider>().isTrigger = true;
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
        ikAnimation.SetIKOn();
        transform.localRotation = Quaternion.Euler(Vector3.Scale(transform.localEulerAngles , new Vector3(0,1,0)));
    }
    
    public void PutDown()
    {
        transform.parent = null;
        //transform.position = HoldingPos.position;
        StartCoroutine(waitForDrop());
        //transform.rotation = Quaternion.Euler(Vector3.Scale(transform.rotation.eulerAngles,new Vector3(0,1,0)));
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(3000 * Vector3.Scale(HoldingPos.position - GameObject.Find("Player_Big").transform.position , new Vector3(1,0,1)));
        GetComponent<Rigidbody>().velocity = Vector3.down * 5;
        transform.Find("putdown").GetComponent<vTriggerGenericAction>().enabled = false;
        transform.Find("putdown").GetComponent<Collider>().enabled = false;
        foreach (var trigger in pickUpTriggers)
        {
            trigger.GetComponent<Collider>().enabled = true;
        }
        ikAnimation.SetIKOff();

    }
    

    IEnumerator waitForDrop()
    {
        yield return new WaitForSeconds(0.1f);
        isFalling = true;
        transform.GetComponent<Collider>().isTrigger = false;
    }

    private void Update()
    {

        if (isFalling)
        {
            /*Ray ray  = new Ray(transform.position, Vector3.down);
            float raycastDist = 0.05f;
            Debug.DrawRay(ray.origin, ray.direction * raycastDist, Color.yellow);
            RaycastHit rayHit = new RaycastHit();
            if (Physics.Raycast(ray, out rayHit,raycastDist))
            {
                if (!rayHit.transform.tag.Contains("Player"))
                {
                    GetComponent<Rigidbody>().isKinematic = true;
                    isFalling = false;
                }


            }*/
            /*if (GetComponent<Rigidbody>().velocity.magnitude < 0.05f)
            {
                fallTime += Time.deltaTime;
                if (fallTime >= 1.0f)
                {
                    GetComponent<Rigidbody>().isKinematic = true;
                    isFalling = false;
                    fallTime = 0;
                }
            }
            else
            {
                fallTime = 0;
            }*/
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
    public void SetOutline(bool isOutline)
    {
        if (isOutline)
        {
            foreach (var mesh in Meshes)
            {
                if (mesh.GetComponents<Outline>() != null)
                {
                    mesh.GetComponent<Outline>().EnableOutline();
                }
            }
        }
        else
        {
            foreach (var mesh in Meshes)
            {
                if (mesh.GetComponents<Outline>() != null)
                {
                    mesh.GetComponent<Outline>().DisableOutline();
                }
            }
        }
       
    }
}
