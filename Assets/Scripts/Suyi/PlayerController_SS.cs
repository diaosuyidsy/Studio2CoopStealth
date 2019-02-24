using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController_SS : MonoBehaviour
{
	[Header("Player Controller is supposed to be either 0 or 1")]
	public int PlayerID;
	public float MaxMoveSpeed = 10f;
	public float MaxLookAtRotationDelta = 1f;
	public float JumpForce = 10f;
	public LayerMask JumpMask;

	#region Player Controller Section
	private Player _player;
	private Rigidbody _rb;
	private Vector3 _moveVector;
	private bool _jump;
	private bool _specialAction;
	private bool _stealthKill;
	private float _distToGround;
	#endregion

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(PlayerID);
		_rb = GetComponent<Rigidbody>();
		_distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;
	}

	// Update is called once per frame
	void Update()
	{
		_getInput();
		_processMovement();
		_processAction();
	}

	private void _getInput()
	{
		Vector3 v = transform.forward * Mathf.Abs(_player.GetAxis("Move Vertical")) * MaxMoveSpeed;
		Vector3 u = transform.right * _player.GetAxis("Move Horizontal") * MaxMoveSpeed;
		_moveVector = v + u;
		_moveVector.y = _rb.velocity.y;
		_jump = _player.GetButtonDown("Jump");
		_specialAction = _player.GetButtonDown("Special Usage");
		_stealthKill = _player.GetButtonDoublePressDown("Stealth Kill");
	}

	private void _processMovement()
	{
		_rb.velocity = _moveVector;
		float rotationAngle = Mathf.Atan2(_rb.velocity.x, _rb.velocity.z) * Mathf.Rad2Deg;
		if (!Mathf.Approximately(rotationAngle, 0f))
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, rotationAngle, 0f), MaxLookAtRotationDelta);
	}

	private void _processAction()
	{
		if (_jump) _rb.AddForce(new Vector3(0f, JumpForce, 0f), ForceMode.Impulse);
		if (_specialAction) EventManager.TriggerEvent("TimeSwitch");
		if (_stealthKill)
		{
			RaycastHit hit;
			if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, 2f))
			{
				if (hit.collider.CompareTag("Enemy"))
					hit.collider.gameObject.SetActive(false);
			}
		}
	}

	public bool IsGrounded()
	{
		RaycastHit hit;
		bool result = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out hit, _distToGround, JumpMask);

		return result;
	}
}
