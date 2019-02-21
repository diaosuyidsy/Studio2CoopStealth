using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotionVelocitySync : MonoBehaviour
{
	private Transform RootTransform;
	private Animator _animator;
	private NavMeshAgent _agent;
	private Vector2 _smoothDeltaPosition = Vector2.zero;
	Vector2 velocity = Vector2.zero;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_agent = GetComponentInParent<NavMeshAgent>();
		RootTransform = transform.parent;
		_agent.updatePosition = false;
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 worldDeltaPosition = _agent.nextPosition - RootTransform.position;

		// Map 'worldDeltaPosition' to local space
		float dx = Vector3.Dot(RootTransform.right, worldDeltaPosition);
		float dy = Vector3.Dot(RootTransform.forward, worldDeltaPosition);
		Vector2 deltaPosition = new Vector2(dx, dy);

		// Low-pass filter the deltaMove
		float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
		_smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, deltaPosition, smooth);

		// Update velocity if time advances
		if (Time.deltaTime > 1e-5f)
			velocity = _smoothDeltaPosition / Time.deltaTime;

		velocity /= 4f;
		// Update animation parameters
		_animator.SetFloat("InputHorizontal", velocity.x);
		_animator.SetFloat("InputVertical", velocity.y);
		_animator.SetFloat("InputMagnitude", velocity.magnitude);
	}

	private void OnAnimatorMove()
	{
		RootTransform.position = _agent.nextPosition;
	}
}
