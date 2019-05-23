using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Interactable_Trigger : Interactable
{
	public string DTAnimationID = "Down";
	public Interactable Object;
	private DOTweenAnimation _dta;

	private void Awake()
	{
		_dta = GetComponent<DOTweenAnimation>();
	}

	public override void OnInteractDown(Object param)
	{
		base.OnInteractDown(param);
		if (_dta != null)
			_dta.DOPlayForwardById(DTAnimationID);
		if (Object != null)
		{
			Object.OnInteractDown(param);
		}
	}

	public override void OnInteractUp(Object param)
	{
		base.OnInteractUp(param);
		if (_dta != null)
			_dta.DOPlayBackwardsById(DTAnimationID);
		if (Object != null) Object.OnInteractUp(param);
	}
}
