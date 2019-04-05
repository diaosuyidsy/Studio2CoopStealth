using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObj : MonoBehaviour
{
    private Transform playerPos;

    private void Start()
    {
        playerPos = GameObject.Find("Player_Big").transform;
    }

    public void MoveObj()
    {
        StartCoroutine(SetParent());
        
        
        transform.GetComponent<Rigidbody>().isKinematic = false;
    }

    IEnumerator SetParent()
    {
        yield return new WaitForSeconds(1f);
        transform.parent = playerPos;
    }
    
    

    public void StopMove()
    {
        StopAllCoroutines();
        transform.parent = null;
    }

}
