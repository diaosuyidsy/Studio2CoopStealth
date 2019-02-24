using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class SetVector3FromGameObject : Action
{
	public SharedGameObject gameobject;
	public SharedVector3 vector3;

	public override TaskStatus OnUpdate()
	{
		if (gameobject != null)
		{
			Vector3 temp = gameobject.Value.transform.position;
			vector3.Value = new Vector3(temp.x, temp.y, temp.z);
		}

		return TaskStatus.Success;
	}
}
