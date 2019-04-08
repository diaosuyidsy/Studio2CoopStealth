using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class DropAwarenessLevel : Action
{
	public SharedFloat DropSpeed = 1f;

	public SharedFloat BarAmount;

	public override TaskStatus OnUpdate()
	{
		if (BarAmount.Value > 0f)
		{
			BarAmount.Value -= Time.deltaTime * DropSpeed.Value;
		}
		else
		{
			BarAmount.Value = 0f;
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}
}
