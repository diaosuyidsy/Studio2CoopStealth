using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_MovePlatform : Interactable_Trigger
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player1") || (other.CompareTag("Player2")))
        {
            OnInteractDown(other.gameObject);
            other.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player1") || (other.CompareTag("Player2")))
        {
            OnInteractUp(other.gameObject);
            other.transform.parent = null;
        }
    }

    public void RemovePlayer()
    {
        while (transform.childCount > 0)
        {
            transform.GetChild(0).parent = null;
        }
    }
}
