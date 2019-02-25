using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastOffset : MonoBehaviour
{
    public Vector3 offset;

    public Transform futureScopeCameraPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        transform.rotation = futureScopeCameraPos.rotation;
        transform.position = futureScopeCameraPos.position + offset;

    }
}
