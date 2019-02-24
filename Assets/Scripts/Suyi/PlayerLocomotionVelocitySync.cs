using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionVelocitySync : MonoBehaviour
{
	private Transform RootTransform;
	private Rigidbody _rb;
	private Animator _animator;
	private Vector2 _smoothDeltaPosition = Vector2.zero;
	Vector3 velocity = Vector2.zero;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_rb = GetComponentInParent<Rigidbody>();
		RootTransform = transform.parent;
	}

	private void Update()
	{
		ConsoleProDebug.Watch("Player Velocity", _rb.velocity.ToString());
		velocity = _rb.velocity;
		_animator.SetFloat("InputHorizontal", velocity.x);
		_animator.SetFloat("InputVertical", velocity.z);
		_animator.SetFloat("InputMagnitude", velocity.magnitude);
	}

}
