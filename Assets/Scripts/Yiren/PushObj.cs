using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObj : MonoBehaviour
{

    public Vector3 keepDis;
    private Transform playerPos;

    private void Start()
    {
        playerPos = GameObject.Find("Player_Big").transform;
    }

    public void MoveObj()
    {

        transform.parent = playerPos;
        transform.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void StopMove()
    {
        transform.parent = null;
    }

}
