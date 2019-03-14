using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Trigger : Interactable
{
	public Interactable Object;

	public override void OnInteractDown(Object param)
	{
		base.OnInteractDown(param);
		Object.OnInteractDown(param);
	}

	public override void OnInteractUp(Object param)
	{
		base.OnInteractUp(param);
		Object.OnInteractUp(param);
	}
}
