using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class RaiseAwarenessLevel : Action
{
	public float MaxFillSpeed = 10f;
	public Image DiscoverBar;
	public SharedGameObject EnemyTarget;
	public SharedGameObject LockedTarget;

	public SharedFloat BarAmount;

	public override void OnStart()
	{
		EnemyTarget.Value = null;
	}

	public override TaskStatus OnUpdate()
	{
		if (BarAmount.Value > 0f)
		{
			BarAmount.Value -= Time.deltaTime * MaxFillSpeed;
			DiscoverBar.fillAmount = (100f - BarAmount.Value) / 100f;
		}
		else
		{
			BarAmount.Value = 0f;
			if (EnemyTarget.Value != null) LockedTarget.Value = EnemyTarget.Value;
			return TaskStatus.Failure;
		}
		return TaskStatus.Running;
	}
}
