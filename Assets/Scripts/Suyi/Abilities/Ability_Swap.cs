using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Ability_Swap : Ability
{
	public float Range = Mathf.Infinity;
	public LayerMask TransportObstacleMask;

	private GameObject _otherPlayer;
	private Vector3 _otherPlayerBodyCenter
	{
		get
		{
			Debug.Assert(_otherPlayer != null);
			Collider col = _otherPlayer.GetComponent<Collider>();
			Debug.Assert(col != null);
			Vector3 pos = _otherPlayer.transform.position;
			pos.y += col.bounds.extents.y;
			return pos;
		}
	}
	private Vector3 _playerBodyCenter
	{
		get
		{
			Collider col = GetComponent<Collider>();
			Debug.Assert(col != null);
			Vector3 pos = transform.position;
			pos.y += col.bounds.extents.y;
			return pos;
		}
	}

	public override void Awake()
	{
		base.Awake();
		_otherPlayer = GameObject.FindGameObjectWithTag("Player1");
	}

	public override void OnPressedDownAbility()
	{
		if (_isUsingOtherAbility) return;
		nextReadyTime = BaseCoolDown + Time.time;
		coolDownTimeLeft = BaseCoolDown;

		// Actual Swap Position Mechanic, Only swap when there is nothing in between
		if (Vector3.Distance(transform.position, _otherPlayer.transform.position) < Range
			&& !Physics.Linecast(_otherPlayerBodyCenter, _playerBodyCenter, TransportObstacleMask))
		{
			Vector3 temp = transform.position;
			transform.position = _otherPlayer.transform.position;
			_otherPlayer.transform.position = temp;
		}
	}

	private void Update()
	{
		bool coolDownComplete = Time.time > nextReadyTime;
		if (coolDownComplete)
		{
			if (_player.GetButtonDown(ButtonName)) OnPressedDownAbility();
		}
		else CoolDown();
	}
}
