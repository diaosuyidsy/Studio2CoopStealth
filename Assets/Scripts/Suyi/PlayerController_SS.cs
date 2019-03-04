using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController_SS : MonoBehaviour
{
	[Header("Player Controller is supposed to be either 0 or 1")]
	public int PlayerID;
	public float MaxMoveSpeed = 10f;
	public float MaxLookAtRotationDelta = 4f;
	public float JumpForce = 10f;
	public LayerMask JumpMask;
	public Transform LookTarget;

	#region Player Controller Section
	private Player _player;
	private Rigidbody _rb;
	private Vector3 _moveVector;
	private float _horizontal;
	private float _vertical;
	private bool _jump;
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
		_horizontal = _player.GetAxis("Move Horizontal");
		_vertical = _player.GetAxis("Move Vertical");
		_jump = _player.GetButtonDown("Jump");
	}

	private void _processMovement()
	{
		_moveVector.x = _horizontal * MaxMoveSpeed;
		_moveVector.y = _rb.velocity.y;
		_moveVector.z = _vertical * MaxMoveSpeed;
		_rb.velocity = _moveVector;
		if (!Mathf.Approximately(_horizontal, 0f) || !Mathf.Approximately(_vertical, 0f))
		{
			Transform target = LookTarget.transform.GetChild(0);
			Vector3 relativePos = target.position - transform.position;

			LookTarget.transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(_horizontal, _vertical) * Mathf.Rad2Deg, transform.eulerAngles.z);

			Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
			Quaternion tr = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * MaxLookAtRotationDelta);
			transform.rotation = tr;
		}
	}

	private void _processAction()
	{
		if (_jump && IsGrounded()) _rb.AddForce(new Vector3(0f, JumpForce, 0f), ForceMode.Impulse);

	}

	public bool IsGrounded()
	{
		RaycastHit hit;
		bool result = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out hit, _distToGround, JumpMask);

		return result;
	}


}
