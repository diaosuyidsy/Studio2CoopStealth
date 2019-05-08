﻿using System.Collections;
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
			if (gameObject.GetComponentInChildren<EnemyShootVisual>() != null)
				gameObject.GetComponentInChildren<EnemyShootVisual>().Shoot(0.2f, LockedTarget.Value.transform);

			LockedTarget.Value = null;
			return TaskStatus.Success;
		}
		else return TaskStatus.Failure;
	}
}
