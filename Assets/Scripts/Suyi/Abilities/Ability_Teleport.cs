using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Teleport : Ability
{
	private GameObject TeleportTransmitter;

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
		var collider = GetComponent<Collider>();
		if (collider != null)
			transform.position = TeleportTransmitter.transform.position + collider.bounds.extents;
		else
			transform.position = TeleportTransmitter.transform.position;
		TeleportTransmitter.SetActive(false);
	}
}
