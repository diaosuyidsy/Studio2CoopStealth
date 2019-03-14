using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    private GameObject movePlatform;
    // Start is called before the first frame update
    void Start()
    {
        movePlatform = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player2"))
        {
            movePlatform.GetComponent<DOTweenAnimation>().DOPlayForwardById("Move");
            other.transform.parent = movePlatform.transform;
        }

        if (other.name.Contains("Pillar"))
        {
            Destroy(movePlatform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player2"))
        {
            movePlatform.GetComponent<DOTweenAnimation>().DOPause();
            other.transform.parent = null;
        }
    }
}
