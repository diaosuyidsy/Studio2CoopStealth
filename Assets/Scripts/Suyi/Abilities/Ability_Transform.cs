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

	public override void Awake()
	{
		base.Awake();
		_MaxEnergyAmount = BaseCoolDown;
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
		EventManager.TriggerEvent($"Player{PlayerID}Stone");
	}

	public override void OnLiftUpAbility()
	{
		EventManager.TriggerEvent($"Player{PlayerID}NotStone");
		nextReadyTime = Time.time + BaseCoolDown;
		coolDownTimeLeft = BaseCoolDown;
	}
}
