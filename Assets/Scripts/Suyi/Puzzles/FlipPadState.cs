using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(DOTweenAnimation))]
public class FlipPadState : MonoBehaviour
{
	private DOTweenAnimation _dta;
	private bool _flipped = false;
	private GameObject ModelAndTrigger;

	private void Awake()
	{
		_dta = GetComponent<DOTweenAnimation>();
		ModelAndTrigger = transform.Find("ModelAndTrigger").gameObject;
		Debug.Assert(ModelAndTrigger != null);
	}

	public void Flip()
	{
		StartCoroutine(flip());
	}

	public void DisableTriggerForSeconds(float time)
	{
		StartCoroutine(disableTrigger(time));
	}

	IEnumerator disableTrigger(float time)
	{
		ModelAndTrigger.SetActive(false);
		yield return new WaitForSeconds(time);
		ModelAndTrigger.SetActive(true);
	}

	IEnumerator flip()
	{
		yield return new WaitForSeconds(0.5f);
		if (GetComponentInChildren<Interactable_FlipPad>().enteredplayernum != 0) yield break;
		if (!_flipped) _dta.DOPlayForward();
		else _dta.DOPlayBackwards();
		_flipped = !_flipped;
	}
}
