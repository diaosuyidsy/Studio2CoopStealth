using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PressButton : MonoBehaviour
{
    
    private GameObject button;
    // Start is called before the first frame update
    void Start()
    {
        button = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player1"))
        {
            button.GetComponent<DOTweenAnimation>().DOPlayForwardById("Down");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player1"))
        {
            button.GetComponent<DOTweenAnimation>().DOPlayBackwardsById("Down");
        }
    }
}
