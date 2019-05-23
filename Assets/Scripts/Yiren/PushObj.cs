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
    public bool isBlockedBackwards;
    Vector3 SpherePosition1;
    Vector3 SpherePosition2;
    Vector3 SpherePosition3;
    public LayerMask pushLayerMask;
    public LayerMask pushBackwardsLayerMask;
    
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
            pushRigidbody.MovePosition(Vector3.Scale(playerPos.position, new Vector3(1, 0, 1)) +
                                       new Vector3(0, transform.position.y, 0) + playerDis);

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
            
            if (Mathf.Abs(playerDis.x) > Mathf.Abs(playerDis.z)) 
            {
                if (playerDis.x < 0) 
                {
                    SpherePosition1 = transform.position + new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.4f,
                                GetComponent<Collider>().bounds.extents.z);
                    SpherePosition2 = transform.position + new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.4f,
                                -GetComponent<Collider>().bounds.extents.z);
                    SpherePosition3 = transform.position + new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.4f,
                                          0);
                    
                }
                else
                {
                    SpherePosition1  =transform.position + new Vector3(GetComponent<Collider>().bounds.extents.x, 0.4f,
                                GetComponent<Collider>().bounds.extents.z);
                    SpherePosition2=transform.position + new Vector3(GetComponent<Collider>().bounds.extents.x, 0.4f,
                                -GetComponent<Collider>().bounds.extents.z);
                    SpherePosition3 = transform.position + new Vector3(GetComponent<Collider>().bounds.extents.x, 0.4f,
                                          0);
                }
                
                
            }
            else
            {
                if (playerDis.z < 0)
                {
                    SpherePosition1  =transform.position + new Vector3(GetComponent<Collider>().bounds.extents.x, 0.4f,
                                -GetComponent<Collider>().bounds.extents.z);
                    SpherePosition2 =transform.position + new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.4f,
                                -GetComponent<Collider>().bounds.extents.z);
                    SpherePosition3 = transform.position + new Vector3(0, 0.4f,
                                          -GetComponent<Collider>().bounds.extents.z);
                }
                else
                {
                    SpherePosition1  =transform.position + new Vector3(GetComponent<Collider>().bounds.extents.x, 0.4f,
                                GetComponent<Collider>().bounds.extents.z);
                    SpherePosition2 =transform.position + new Vector3(-GetComponent<Collider>().bounds.extents.x, 0.4f,
                                GetComponent<Collider>().bounds.extents.z);
                    SpherePosition3 = transform.position + new Vector3(0, 0.4f,
                                          GetComponent<Collider>().bounds.extents.z);
                }
                
            }
            Collider[] hitColliders1 = Physics.OverlapSphere(SpherePosition1, 0.3f,pushLayerMask);
            Collider[] hitColliders2 = Physics.OverlapSphere(SpherePosition2, 0.3f,pushLayerMask);
            Collider[] hitColliders3 = Physics.OverlapSphere(playerPos.position + new Vector3(0,1.2f,0), 1.0f,pushBackwardsLayerMask);
            Collider[] hitColliders4 = Physics.OverlapSphere(SpherePosition3, 0.3f, pushLayerMask);
            RaycastHit forwardRayHit = new RaycastHit();
            if (hitColliders1.Length> 0 || hitColliders2.Length > 0 || hitColliders4.Length> 0)
            {

                isBlocked = true;

            }
            else
            {
                isBlocked = false;
            }

            if (hitColliders3.Length > 0)
            {
                isBlockedBackwards = true;
            }
            else
            {
                isBlockedBackwards = false;
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
        Gizmos.DrawSphere(SpherePosition1, 0.3f);
        Gizmos.DrawSphere(SpherePosition2, 0.3f);
        Gizmos.DrawSphere(playerPos.position + new Vector3(0,1.2f,0), 1.0f);
    }
}
