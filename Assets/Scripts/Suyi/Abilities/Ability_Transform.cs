using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// This Ability's Cooldown only counts when ability is lifted up
/// </summary>
public class Ability_Transform : Ability
{
	public GameObject StoneForm;
	public GameObject PlayerModel;

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
		if (_isUsingOtherAbility) return;
		EventManager.TriggerEvent($"Player{PlayerID}InAbility");
		StoneForm.SetActive(true);
		PlayerModel.SetActive(false);
	}

	public override void OnLiftUpAbility()
	{
		if (!StoneForm.activeSelf) return;
		EventManager.TriggerEvent($"Player{PlayerID}Free");
		StoneForm.SetActive(false);
		PlayerModel.SetActive(true);
		nextReadyTime = Time.time + BaseCoolDown;
		coolDownTimeLeft = BaseCoolDown;
	}
}
