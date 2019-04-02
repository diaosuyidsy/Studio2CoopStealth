using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class KillTarget : Action
{
	public SharedGameObject LockedTarget;

	public override TaskStatus OnUpdate()
	{
		if (LockedTarget.Value.activeSelf)
		{
			LockedTarget.Value.SetActive(false);
			LockedTarget.Value = null;
			EventManager.TriggerEvent("PlayerDied");
			return TaskStatus.Success;
		}
		else return TaskStatus.Failure;
	}
}
