using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController.vActions;
using cakeslice;
using Invector.vCharacterController;
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
    protected vThirdPersonInput tpInput;
    private bool isPutDown;
    private GameObject PlayerBig;
    private void Start()
    {
        PlayerBig = GameObject.FindGameObjectWithTag("Player1");
        ikAnimation = PlayerBig.GetComponent<IKAnimation>();
        HoldingPos = PlayerBig.transform.Find("hip").Find("HoldingPos");
        tpInput = PlayerBig.GetComponent<vThirdPersonInput>(); 
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
        StartCoroutine(waitForPick());
        
    }
    
    public void PutDown()
    {
        tpInput.cc.animator.SetInteger("ActionState", 0);
        tpInput.cc.stopMove = true;
        isPutDown = true;
        
        transform.parent = null;
        //transform.position = HoldingPos.position;
        StartCoroutine(waitForDrop());
        transform.rotation = Quaternion.Euler(Vector3.Scale(transform.rotation.eulerAngles,new Vector3(0,1,0)));
        GetComponent<Rigidbody>().isKinematic = false;
        //GetComponent<Rigidbody>().AddForce(3000 * Vector3.Scale(HoldingPos.position - GameObject.Find("Player_Big").transform.position , new Vector3(1,0,1)));
        GetComponent<Rigidbody>().velocity = Vector3.down * 10;
        transform.Find("putdown").GetComponent<vTriggerGenericAction>().enabled = false;
        transform.Find("putdown").GetComponent<Collider>().enabled = false;
        foreach (var trigger in pickUpTriggers)
        {
            trigger.GetComponent<Collider>().enabled = true;
        }
        ikAnimation.SetIKOff();
        

    }

    IEnumerator waitForPick()
    {
        yield return new WaitForSeconds(0.6f);
        PlayerBig.GetComponent<Ability_Assasination>().enabled = false;
        PlayerBig.GetComponent<Ability_Interact>().enabled = false;
        PlayerBig.GetComponent<Ability_ThrowTransmitter>().enabled = false;

    }

    IEnumerator waitForDrop()
    {
        PlayerBig.GetComponent<Ability_Interact>().enabled = true;
        PushAction pushAction = PlayerBig.GetComponent<PushAction>();
        
        if (pushAction.isUnlockAssasin)
        {
            PlayerBig.GetComponent<Ability_Assasination>().enabled = true;
        }

        if (pushAction.isUnlockThrowTransmitter)
        {
            PlayerBig.GetComponent<Ability_ThrowTransmitter>().enabled = true;
        }
        yield return new WaitForSeconds(0.1f);
        isFalling = true;
        transform.GetComponent<Collider>().isTrigger = false;
        isPutDown = false;
    }

    private void Update()
    {
        if (isPutDown)
        {
            tpInput.cc.stopMove = true;
            
        }
        if (isFalling)
        {
            Ray ray  = new Ray(transform.position, Vector3.down);
            float raycastDist = 0.05f;
            Debug.DrawRay(ray.origin, ray.direction * raycastDist, Color.yellow);
            RaycastHit rayHit = new RaycastHit();
            if (Physics.Raycast(ray, out rayHit,raycastDist))
            {
                if (!rayHit.transform.tag.Contains("Player") && rayHit.transform.gameObject.layer != LayerMask.NameToLayer("Interactable"))
                {
                    GetComponent<Rigidbody>().isKinematic = true;
                    isFalling = false;
                }


            }
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
