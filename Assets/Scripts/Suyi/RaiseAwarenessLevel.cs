using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class RaiseAwarenessLevel : Action
{
	public float FillSpeed = 10f;
	public Image DiscoverBar;

	private SharedFloat BarAmount;

	public override void OnStart()
	{
		DiscoverBar.fillAmount = 0f;
		BarAmount = 100f;
	}

	public override TaskStatus OnUpdate()
	{
		if (BarAmount.Value > 0f)
		{
			BarAmount.Value -= Time.deltaTime * FillSpeed;
			DiscoverBar.fillAmount = (100f - BarAmount.Value) / 100f;
		}
		else
		{
			DiscoverBar.fillAmount = 0f;
			return TaskStatus.Failure;
		}
		return TaskStatus.Running;
	}
}
