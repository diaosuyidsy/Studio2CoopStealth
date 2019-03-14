using System.Collections;
using System.Collections.Generic;
using Invector.vShooter;
using UnityEngine;
using Rewired;

public class Interactable_TriggerControlpad : Interactable
{
	public float MoveSpeed = 2f;
	public Vector3 PadCenter;
	public Vector3 PadMovementSize = new Vector3(5f, 0f, 5f);

	private Player _curPlayer;
	private Rigidbody _rb;
	private Vector3 _moveVector;
	private float _xmin;
	private float _xmax;
	private float _zmin;
	private float _zmax;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_xmin = PadCenter.x + GetComponent<Collider>().bounds.extents.x - PadMovementSize.x / 2;
		_xmax = PadCenter.x - GetComponent<Collider>().bounds.extents.x + PadMovementSize.x / 2;
		_zmin = PadCenter.z + GetComponent<Collider>().bounds.extents.z - PadMovementSize.z / 2;
		_zmax = PadCenter.z - GetComponent<Collider>().bounds.extents.z + PadMovementSize.z / 2;
	}

	private void Update()
	{
		if (state == InteractableState.Interacting && _curPlayer != null)
		{
			float haxis = _curPlayer.GetAxis("Move Horizontal");
			float vaxis = _curPlayer.GetAxis("Move Vertical");
			_moveVector.x = haxis * MoveSpeed;
			_moveVector.z = vaxis * MoveSpeed;
			_rb.velocity = _moveVector;
			// Clamp Pad Position
			Vector3 _curPosition = transform.position;
			_curPosition.x = Mathf.Clamp(_curPosition.x, _xmin, _xmax);
			_curPosition.z = Mathf.Clamp(_curPosition.z, _zmin, _zmax);
			transform.position = _curPosition;
		}
	}

	public override void OnInteractDown(Object param)
	{
		base.OnInteractDown(param);
		var curInput = ((GameObject)param).GetComponent<vShooterMeleeInput>();
		if (curInput)
			_curPlayer = ReInput.players.GetPlayer(curInput.playerId);
	}

	public override void OnInteractUp(Object param)
	{
		base.OnInteractUp(param);
		_rb.velocity = Vector3.zero;
		_curPlayer = null;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);
		Gizmos.DrawCube(PadCenter, PadMovementSize);
	}
}
