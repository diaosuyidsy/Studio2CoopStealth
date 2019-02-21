using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class LookAt : Action
{
	public SharedGameObject Target;

	public override TaskStatus OnUpdate()
	{
		if (Target.Value != null)
		{
			transform.LookAt(Target.Value.transform);
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}
}
