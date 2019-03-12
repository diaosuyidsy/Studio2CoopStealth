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
			transform.LookAt(new Vector3(Target.Value.transform.position.x, transform.position.y, Target.Value.transform.position.z));
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}
}
