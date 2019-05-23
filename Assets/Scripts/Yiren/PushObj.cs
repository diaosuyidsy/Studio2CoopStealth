using System.Collections;
using System.Collections.Generic;
using cakeslice;
using Invector.vCharacterController.vActions;
using UnityEngine;

public class PushObj : MonoBehaviour
{
    public List<GameObject> Meshes;
    private Transform playerPos;
    private bool isPushing = false;
    private Rigidbody pushRigidbody;
    private Vector3 playerDis;

    private Transform target;
    public bool isFall = false;
    public bool isBlocked;
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
        print(playerDis);
    }

    public void DoNotPush()
    {
        isPushing = false;
    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDied", DoNotPush);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDied", DoNotPush);
    }

    private void Update()
    {
        if (isPushing)
        {
            pushRigidbody.MovePosition(Vector3.Scale(playerPos.position, new Vector3(1,0,1)) + new Vector3(0, transform.position.y, 0) + playerDis);
            //Raycast before object
            /*Ray forwardRay1;
            Ray forwardRay2;
            if (Mathf.Abs(playerDis.x) > Mathf.Abs(playerDis.z))
            {
                if (playerDis.x < 0)
                {
                    forwardRay1 =
                        new Ray(
                            transform.position + new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.3f,
                                GetComponent<Collider>().bounds.extents.z), playerDis.normalized);
                    forwardRay2 =
                        new Ray(
                            transform.position + new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.3f,
                                -GetComponent<Collider>().bounds.extents.z), playerDis.normalized);
                }
                else
                {
                    forwardRay1 =
                        new Ray(
                            transform.position + new Vector3(GetComponent<Collider>().bounds.extents.x, 0.3f,
                                GetComponent<Collider>().bounds.extents.z), playerDis.normalized);
                    forwardRay2 =
                        new Ray(
                            transform.position + new Vector3(GetComponent<Collider>().bounds.extents.x, 0.3f,
                                -GetComponent<Collider>().bounds.extents.z), playerDis.normalized);
                }
                
            }
            else
            {
                if (playerDis.z < 0)
                {
                    forwardRay1 =
                        new Ray(
                            transform.position + new Vector3(GetComponent<Collider>().bounds.extents.x, 0.3f,
                                -GetComponent<Collider>().bounds.extents.z), playerDis.normalized);
                    forwardRay2 =
                        new Ray(
                            transform.position + new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.3f,
                                -GetComponent<Collider>().bounds.extents.z), playerDis.normalized);
                }
                else
                {
                    forwardRay1 =
                        new Ray(
                            transform.position + new Vector3(GetComponent<Collider>().bounds.extents.x, 0.3f,
                                GetComponent<Collider>().bounds.extents.z), playerDis.normalized);
                    forwardRay2 =
                        new Ray(
                            transform.position + new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.3f,
                                GetComponent<Collider>().bounds.extents.z), playerDis.normalized);
                }
                
            }
                       
            float forwardRaycastDist = 0.3f;
            Debug.DrawRay(forwardRay1.origin, forwardRay1.direction * forwardRaycastDist, Color.green);
            Debug.DrawRay(forwardRay2.origin, forwardRay2.direction * forwardRaycastDist, Color.green);
            RaycastHit forwardRayHit = new RaycastHit();
            if (Physics.Raycast(forwardRay1, out forwardRayHit,forwardRaycastDist) || Physics.Raycast(forwardRay2, out forwardRayHit,forwardRaycastDist))
            {

                    isBlocked = true;

            }
            else
            {
                    isBlocked = false;
            }*/
            Vector3 SpherePosition1;
            Vector3 SpherePosition2;
            if (Mathf.Abs(playerDis.x) > Mathf.Abs(playerDis.z))
            {
                if (playerDis.x < 0)
                {
                    SpherePosition1 = new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.3f,
                                GetComponent<Collider>().bounds.extents.z);
                    SpherePosition2 =new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.3f,
                                -GetComponent<Collider>().bounds.extents.z);
                }
                else
                {
                    SpherePosition1  =new Vector3(GetComponent<Collider>().bounds.extents.x, 0.3f,
                                GetComponent<Collider>().bounds.extents.z);
                    SpherePosition2=new Vector3(GetComponent<Collider>().bounds.extents.x, 0.3f,
                                -GetComponent<Collider>().bounds.extents.z);
                }
                
            }
            else
            {
                if (playerDis.z < 0)
                {
                    SpherePosition1  =new Vector3(GetComponent<Collider>().bounds.extents.x, 0.3f,
                                -GetComponent<Collider>().bounds.extents.z);
                    SpherePosition2 =new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.3f,
                                -GetComponent<Collider>().bounds.extents.z);
                }
                else
                {
                    SpherePosition1  =new Vector3(GetComponent<Collider>().bounds.extents.x, 0.3f,
                                GetComponent<Collider>().bounds.extents.z);
                    SpherePosition2 =new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.3f,
                                GetComponent<Collider>().bounds.extents.z);
                }
                
            }
            RaycastHit forwardRayHit = new RaycastHit();
            if (Physics.SphereCast(SpherePosition1, 0.3f, playerDis.normalized, out forwardRayHit, 10) || Physics.SphereCast(SpherePosition2, 0.3f, playerDis.normalized, out forwardRayHit, 10))
            {

                isBlocked = true;

            }
            else
            {
                isBlocked = false;
            }
        }
        
            
        /*Ray ray  = new Ray(transform.position - new Vector3(0,-0.1f,0) - playerDis.normalized * 0.6f, Vector3.down);
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
        }*/
        
    }

    public void StopMove()
    {
        isPushing = false;
        StopAllCoroutines();
        //transform.parent = null;
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
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
