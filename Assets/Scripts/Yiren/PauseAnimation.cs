using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PauseAnimation : MonoBehaviour
{


    public void PauseForSeconds(float time)
    {
        GetComponent<DOTweenAnimation>().DOPause();
        StartCoroutine(WaitPause(time));
    }

    IEnumerator WaitPause(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<DOTweenAnimation>().DOPlayBackwards();
    }
}
