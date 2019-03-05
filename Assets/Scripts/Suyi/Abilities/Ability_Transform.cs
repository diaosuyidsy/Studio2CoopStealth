using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// This Ability's Cooldown only counts when ability is lifted up
/// </summary>
public class Ability_Transform : Ability
{
	private float _MaxEnergyAmount;
	private readonly int PlayerID = 0;
	private Player _player;

	private void Awake()
	{
		_MaxEnergyAmount = BaseCoolDown;
		_player = ReInput.players.GetPlayer(PlayerID);
	}

	private void Update()
	{
		bool coolDownComplete = Time.time > nextReadyTime;
		if (coolDownComplete)
		{
			if (_player.GetButtonDown(ButtonName)) OnPressedDownAbility();
			if (_player.GetButtonUp(ButtonName)) OnLiftUpAbility();
		}
		else CoolDown();
	}

	public override void OnPressedDownAbility()
	{
	}

	public override void OnLiftUpAbility()
	{
		nextReadyTime = Time.time + BaseCoolDown;
		coolDownTimeLeft = BaseCoolDown;
	}
}
