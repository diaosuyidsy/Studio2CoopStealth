using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Teleport : Ability
{
	private GameObject TeleportTransmitter;

	public override void Awake()
	{
		base.Awake();
		TeleportTransmitter = GameObject.FindGameObjectWithTag("TeleportTransmitter");
		TeleportTransmitter.SetActive(false);
	}

	private void Update()
	{
		if (_player.GetButtonDown(ButtonName))
		{
			OnPressedDownAbility();
		}
	}

	public override void OnPressedDownAbility()
	{
		if (!TeleportTransmitter.activeSelf) return;
		transform.position = TeleportTransmitter.transform.position;
		TeleportTransmitter.SetActive(false);
	}
}
