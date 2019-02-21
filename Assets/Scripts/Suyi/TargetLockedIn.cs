using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class TargetLockedIn : Conditional
{
	public SharedGameObject Target;

	public override TaskStatus OnUpdate()
	{
		if (Target.Value != null)
			return TaskStatus.Success;
		return TaskStatus.Failure;
	}
}
