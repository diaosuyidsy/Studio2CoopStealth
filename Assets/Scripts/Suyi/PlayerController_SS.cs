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
	private bool _stealthKill;
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
		_stealthKill = _player.GetButtonDoublePressDown("Stealth Kill");
	}

	private void _processMovement()
	{
		_rb.velocity = _moveVector;
		//transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(_moveVector.x, _moveVector.z * -1f) * Mathf.Rad2Deg, transform.eulerAngles.z);
	}

	private void _processAction()
	{
		if (_jump) _rb.AddForce(new Vector3(0f, JumpForce, 0f), ForceMode.Impulse);
		if (_specialAction) EventManager.TriggerEvent("TimeSwitch");
		if (_stealthKill)
		{
			RaycastHit hit;
			if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, 1f))
			{
				if (hit.collider.CompareTag("Enemy"))
					hit.collider.gameObject.SetActive(false);
			}
		}
	}
}
