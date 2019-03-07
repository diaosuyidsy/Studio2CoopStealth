using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushObj : MonoBehaviour
{

    public Vector3 keepDis;
    public void MoveObj(Transform PlayerPosition)
    {

        transform.parent = PlayerPosition;
        transform.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void StopMove()
    {
        transform.parent = null;
    }

}
