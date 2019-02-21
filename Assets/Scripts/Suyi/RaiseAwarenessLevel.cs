using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class RaiseAwarenessLevel : Action
{
	public float MaxFillSpeed = 10f;
	public SharedGameObject DiscoverBar;
	public SharedGameObject EnemyTarget;
	public SharedFloat BarAmount;
	public float BarSuccessAmount = 100f;


	public override void OnStart()
	{
		EnemyTarget.Value = null;
	}

	public override TaskStatus OnUpdate()
	{
		if (BarAmount.Value > (100f - BarSuccessAmount))
		{
			BarAmount.Value -= Time.deltaTime * MaxFillSpeed;
			DiscoverBar.Value.GetComponent<Image>().fillAmount = (100f - BarAmount.Value) / 100f;
		}
		else
		{
			BarAmount.Value = 0f;
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}
}
