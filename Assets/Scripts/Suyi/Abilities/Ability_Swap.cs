using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Ability_Swap : Ability
{
	public float Range = Mathf.Infinity;

	private GameObject _otherPlayer;

	public override void Awake()
	{
		base.Awake();
		_otherPlayer = GameObject.FindGameObjectWithTag("Player1");
	}

	public override void OnPressedDownAbility()
	{
		nextReadyTime = BaseCoolDown + Time.time;
		coolDownTimeLeft = BaseCoolDown;

		// Actual Swap Position Mechanic
		if (Vector3.Distance(transform.position, _otherPlayer.transform.position) < Range)
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
