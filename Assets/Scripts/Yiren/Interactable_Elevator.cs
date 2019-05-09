using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Interactable_Elevator : MonoBehaviour
{
    public bool P1Entered;
    public bool P2Entered;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1")) P1Entered = true;
        if (other.CompareTag("Player2")) P2Entered = true;
        if (P1Entered && P2Entered)
        {
            GetComponent<DOTweenAnimation>().DOPlayBackwards();
        }
        
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1")) P1Entered = false;
        if (other.CompareTag("Player2")) P2Entered = false;
        
    }
}
