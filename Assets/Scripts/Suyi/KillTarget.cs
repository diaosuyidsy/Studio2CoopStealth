using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class KillTarget : Action
{
	public SharedGameObject Target;

	public override TaskStatus OnUpdate()
	{
		if (Target.Value.activeSelf)
		{
			Target.Value.SetActive(false);
			return TaskStatus.Success;
		}
		return TaskStatus.Running;
	}
}
