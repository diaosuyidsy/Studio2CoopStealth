using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

public class Ability_Teleport : Ability
{
	private GameObject TeleportTransmitter;
	private Renderer _playerRenderer;
	public bool isTransmittingPhase1;
	public bool isTransmittingPhase2;
	private float _slicePlayer;
	public override void Awake()
	{
		base.Awake();
		TeleportTransmitter = GameObject.FindGameObjectWithTag("TeleportTransmitter");
		TeleportTransmitter.SetActive(false);
		_playerRenderer = transform.Find("girl").GetComponent<Renderer>();
		_slicePlayer = -3f;
	}

	private void Update()
	{
		if (_player.GetButtonDown(ButtonName))
		{
			OnPressedDownAbility();

		}

		if (TeleportTransmitter.activeSelf)
		{
			GetComponent<Ability_Swap>().SwapLine.GetComponent<EGA_Laser>().Connect();
		}
		if (isTransmittingPhase1)
		{
			_slicePlayer += 20 * Time.deltaTime;
			_playerRenderer.material.SetFloat("_Slice", _slicePlayer);

			if (_slicePlayer >= 5)
			{
				GetComponent<Ability_Swap>().SwapLine.GetComponent<EGA_Laser>().Transmit();
				isTransmittingPhase1 = false;


			}
		}
		else if (isTransmittingPhase2)
		{
			transform.position = TeleportTransmitter.transform.position;
			TeleportTransmitter.SetActive(false);
			_slicePlayer -= 20 * Time.deltaTime;
			_playerRenderer.material.SetFloat("_Slice", _slicePlayer);
			if (_slicePlayer <= -3)
			{
				GetComponent<vThirdPersonInput>().enabled = true;
				GetComponent<vThirdPersonInput>().cc.enabled = true;
				isTransmittingPhase2 = false;
			}
		}
	}

	public override void OnPressedDownAbility()
	{
		if (!TeleportTransmitter.activeSelf) return;
		//var collider = GetComponent<Collider>();
		//if (collider != null)
		//	transform.position = TeleportTransmitter.transform.position + collider.bounds.extents;
		//else
		if (!SpendEnergy()) return;
		isTransmittingPhase1 = true;
		GetComponent<vThirdPersonInput>().enabled = false;
		GetComponent<vThirdPersonInput>().cc.enabled = false;
	}
}
