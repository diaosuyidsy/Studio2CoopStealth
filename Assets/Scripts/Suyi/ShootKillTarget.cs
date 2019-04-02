using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class ShootKillTarget : Action
{
	public SharedGameObject LockedTarget;

	public override TaskStatus OnUpdate()
	{
		if (LockedTarget.Value.activeSelf)
		{
			if (GetComponent<EnemyShootVisual>() != null)
				GetComponent<EnemyShootVisual>().Shoot();
			LockedTarget.Value.SetActive(false);
			EventManager.TriggerEvent("PlayerDied");
			LockedTarget.Value = null;
			return TaskStatus.Success;
		}
		else return TaskStatus.Failure;
	}
}
