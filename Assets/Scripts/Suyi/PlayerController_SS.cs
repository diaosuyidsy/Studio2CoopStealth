using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController_SS : MonoBehaviour
{
	[Header("Player Controller is supposed to be either 0 or 1")]
	public int PlayerID;
	public float MoveSpeed = 10f;
	public float JumpForce = 10f;

	#region Player Controller Section
	private Player _player;
	private Rigidbody _rb;
	private Vector3 _moveVector;
	private bool _jump;
	private bool _specialAction;
	#endregion

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(PlayerID);
		_rb = GetComponent<Rigidbody>();
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
		_moveVector.x = _player.GetAxis("Move Horizontal") * MoveSpeed;
		_moveVector.y = _rb.velocity.y;
		_moveVector.z = _player.GetAxis("Move Vertical") * MoveSpeed;
		_jump = _player.GetButtonDown("Jump");
		_specialAction = _player.GetButtonDown("Special Usage");
	}

	private void _processMovement()
	{
		_rb.velocity = _moveVector;
	}

	private void _processAction()
	{
		if (_jump) _rb.AddForce(new Vector3(0f, JumpForce, 0f), ForceMode.Impulse);
		if (_specialAction) EventManager.TriggerEvent("TimeSwitch");
	}
}
