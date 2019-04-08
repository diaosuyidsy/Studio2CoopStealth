using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class RaiseAwarenessLevel : Action
{
	public SharedFloat MaxFillSpeed = 10f;
	public SharedGameObject Player;
	public SharedGameObject EnemyTarget;
	public SharedFloat BarAmount;
	public SharedFloat BarSuccessPercentage = 1f;


	public override void OnStart()
	{
		EnemyTarget.Value = null;
	}

	public override TaskStatus OnUpdate()
	{
		float distanceToPlayer = 0f;
		Vector3 newSelfPos = new Vector3(transform.position.x, 0f, transform.position.z);
		Vector3 newPlayerPos = new Vector3(Player.Value.transform.position.x, 0f, Player.Value.transform.position.z);
		distanceToPlayer = Vector3.Distance(newSelfPos, newPlayerPos);
		if (BarAmount.Value < distanceToPlayer * BarSuccessPercentage.Value)
			BarAmount.Value += Time.deltaTime * MaxFillSpeed.Value;
		else return TaskStatus.Success;
		return TaskStatus.Running;
	}
}
