using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionVelocitySync : MonoBehaviour
{
	private Transform RootTransform;
	private Rigidbody _rb;
	private Animator _animator;
	private Vector2 _smoothDeltaPosition = Vector2.zero;
	private PlayerController_SS _playercontroller;
	Vector3 velocity = Vector2.zero;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_rb = GetComponentInParent<Rigidbody>();
		RootTransform = transform.parent;
		_playercontroller = GetComponentInParent<PlayerController_SS>();
	}

	private void Update()
	{
		velocity = _rb.velocity;
		velocity /= 4f;
		_animator.SetFloat("InputHorizontal", RootTransform.InverseTransformDirection(velocity).x);
		_animator.SetFloat("InputVertical", RootTransform.InverseTransformDirection(velocity).z);
		_animator.SetFloat("InputMagnitude", velocity.magnitude);
		bool isOnGround = _playercontroller.IsGrounded();
		_animator.SetBool("IsGrounded", isOnGround);
	}

}
