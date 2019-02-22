using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class DropAwarenessLevel : Action
{
	public float DropSpeed = 10f;
	public SharedGameObject DiscoverBar;

	public SharedFloat BarAmount;

	public override TaskStatus OnUpdate()
	{
		if (BarAmount.Value < 100f)
		{
			BarAmount.Value += Time.deltaTime * DropSpeed;
			DiscoverBar.Value.GetComponent<Image>().fillAmount = (100f - BarAmount.Value) / 100f;
		}

		return TaskStatus.Running;
	}
}
