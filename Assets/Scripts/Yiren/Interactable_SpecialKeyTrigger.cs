using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_SpecialKeyTrigger : Interactable_Trigger
{
    public string KeyTag;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(KeyTag))
        {
            OnInteractDown(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(KeyTag))
        {
            OnInteractUp(other.gameObject);
        }
    }
}
