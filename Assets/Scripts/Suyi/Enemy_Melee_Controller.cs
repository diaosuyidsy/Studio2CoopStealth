using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime.Tasks.Movement;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_Melee_Controller : MonoBehaviour
{
	public AIState State;

	[Header("Enemy numeric status")]
	#region Status
	public float WalkSpeed = 10f;
	public float PatrolWaitTime = 1f;
	public string PlayerTag = "Player";
	public float FieldViewAngle = 90f;
	public float ViewDistance = 90f;
	public LayerMask PlayerLayerMask;
	#endregion

	[Header("Initialize Enemy Parts into this")]
	#region Initializer
	public Transform PatrolHolder;
	#endregion

	#region Private Variable
	private List<Vector3> _patrolPoints;
	private int _destPoint = 0;
	private NavMeshAgent _agent;
	private float _waypointreachtime = -1f;
	private GameObject _player;
	#endregion

	private void Awake()
	{
		State = AIState.Normal;
		_agent = GetComponent<NavMeshAgent>();
		_agent.autoBraking = false;
		_player = GameObject.FindGameObjectWithTag(PlayerTag);
		_patrolPoints = new List<Vector3>();
		for (int i = 0; i < PatrolHolder.childCount; i++)
		{
			var pp = PatrolHolder.GetChild(i).position;
			_patrolPoints.Add(new Vector3(pp.x, pp.y, pp.z));
		}
		_goToNextPoint();
	}

	// Update is called once per frame
	void Update()
	{
		switch (State)
		{
			case AIState.Idle:
				break;
			case AIState.Normal:
				_patrolBehavior();
				break;
			case AIState.Suspicious:
				break;
			case AIState.LockedIn:
				break;
			default:
				break;
		}
	}

	private void _patrolBehavior()
	{
		// If agent has arrived at a point
		if (!_agent.pathPending && _agent.remainingDistance <= 0.5f)
		{
			if (Mathf.Approximately(-1f, _waypointreachtime))
			{
				_agent.isStopped = true;
				_waypointreachtime = Time.time;
			}
			if (_waypointreachtime + PatrolWaitTime <= Time.time)
			{
				_goToNextPoint();
				_waypointreachtime = -1f;
			}
		}
	}

	#region Helper Functions
	private void _goToNextPoint()
	{
		if (_patrolPoints.Count <= 0) return;
		_agent.isStopped = false;
		_agent.speed = WalkSpeed;
		_agent.destination = _patrolPoints[_destPoint];
		_destPoint = (_destPoint + 1) % _patrolPoints.Count;
	}

	private bool _canSeePlayer()
	{
		GameObject go = MovementUtility.WithinSight(transform, Vector3.zero, FieldViewAngle, ViewDistance, _player, Vector3.zero, PlayerLayerMask, false, HumanBodyBones.Hips);
		if (go != null) return true;
		return false;
	}

	// Draw a gizmo indicating a patrol 
	public void OnDrawGizmos()
	{
#if UNITY_EDITOR
		if (_patrolPoints == null) return;
		var oldColor = UnityEditor.Handles.color;
		UnityEditor.Handles.color = Color.yellow;
		for (int i = 0; i < _patrolPoints.Count; i++)
		{
			if (_patrolPoints[i] != null)
			{
				UnityEditor.Handles.SphereHandleCap(0, _patrolPoints[i], Quaternion.identity, 1, EventType.Repaint);
			}
		}
		UnityEditor.Handles.color = oldColor;
		MovementUtility.DrawLineOfSight(transform, Vector3.zero, FieldViewAngle, 0f, ViewDistance, false);
#endif
	}
	#endregion
}
