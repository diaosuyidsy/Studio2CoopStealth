using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;

public class ObjectInFieldOfView : Conditional
{
	public SharedFloat fieldOfViewAngle = 90;
	public SharedFloat viewDistance = 100;
	public LayerMask TargetLayerMask;
	public LayerMask ObstacleLayerMask;
	public SharedGameObject returnedObject;

	public override TaskStatus OnUpdate()
	{
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewDistance.Value, TargetLayerMask);
		GameObject minTar = null;
		float minDistance = Mathf.Infinity;
		foreach (var target in targetsInViewRadius)
		{
			Transform t = target.transform;
			Vector3 dirToTarget = (t.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < fieldOfViewAngle.Value / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, t.position);
				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, ObstacleLayerMask))
				{
					// meaning target now is a visible enemy
					if (dstToTarget < minDistance)
					{
						minDistance = dstToTarget;
						minTar = target.gameObject;
					}
				}
			}
		}
		if (minTar != null)
		{
			returnedObject.Value = minTar;
			return TaskStatus.Success;
		}

		return TaskStatus.Failure;
	}

	// Draw the line of sight representation within the scene window
	public override void OnDrawGizmos()
	{
		MovementUtility.DrawLineOfSight(Owner.transform, Vector3.zero, fieldOfViewAngle.Value, 0f, viewDistance.Value, false);
	}

	public override void OnBehaviorComplete()
	{
		MovementUtility.ClearCache();
	}
}
