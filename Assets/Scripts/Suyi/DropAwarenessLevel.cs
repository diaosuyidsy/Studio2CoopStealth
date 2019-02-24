using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class DropAwarenessLevel : Action
{
	public SharedFloat DropSpeed = 10f;
	public SharedGameObject DiscoverBar;
	public float BarSuccessAmount = 0f;

	public SharedFloat BarAmount;

	public override TaskStatus OnUpdate()
	{
		if (BarAmount.Value < (100f - BarSuccessAmount))
		{
			BarAmount.Value += Time.deltaTime * DropSpeed.Value;
			DiscoverBar.Value.GetComponent<Image>().fillAmount = (100f - BarAmount.Value) / 100f;
		}
		else
		{
			BarAmount.Value = 100f - BarSuccessAmount;
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}
}
