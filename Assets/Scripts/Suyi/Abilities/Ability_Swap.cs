using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;
using Rewired;

public class Ability_Swap : Ability
{
	public float Range = Mathf.Infinity;
	public LayerMask TransportObstacleMask;
	public GameObject SwapLine;
	private GameObject _otherPlayer;
	public bool isSwapingPhase1;
	public bool isSwapingPhase2;
	private Renderer _playerRenderer;
	private Renderer _otherPlayerRenderer;

	private float _slicePlayer;
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
		_playerRenderer = transform.Find("girl").GetComponent<Renderer>();
		_otherPlayerRenderer = _otherPlayer.transform.Find("Sphere.012").GetComponent<Renderer>();
		_slicePlayer = -3f;
	}

	public override void OnPressedDownAbility()
	{
		if (_isUsingOtherAbility) return;
		

		// Actual Swap Position Mechanic, Only swap when there is nothing in between
		if (Vector3.Distance(transform.position, _otherPlayer.transform.position) < Range
			&& !Physics.Linecast(_otherPlayerBodyCenter, _playerBodyCenter, TransportObstacleMask))
		{
			if (!SpendEnergy()) return;
			nextReadyTime = BaseCoolDown + Time.time;
			coolDownTimeLeft = BaseCoolDown;
			GetComponent<vThirdPersonInput>().enabled = false;
			GetComponent<vThirdPersonInput>().cc.enabled = false;
			_otherPlayer.GetComponent<vThirdPersonInput>().enabled = false;
			_otherPlayer.GetComponent<vThirdPersonInput>().cc.enabled = false;
			isSwapingPhase1 = true;
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

		if (!Physics.Linecast(_otherPlayerBodyCenter, _playerBodyCenter, TransportObstacleMask))
		{
			SwapLine.GetComponent<EGA_Laser>().Connect();
		}
		else
		{
			if (!_otherPlayer.GetComponent<Ability_ThrowTransmitter>().TeleportTransmitter.gameObject.activeSelf)
			{
				SwapLine.GetComponent<EGA_Laser>().Disconnect();
			}
		}

		if (isSwapingPhase1)
		{
			_slicePlayer += 20 * Time.deltaTime;
			_playerRenderer.material.SetFloat("_Slice", _slicePlayer);
			_otherPlayerRenderer.material.SetFloat("_Slice", _slicePlayer);
			if (_slicePlayer >= 5)
			{
				Vector3 temp = transform.position;
				transform.position = _otherPlayer.transform.position;
				_otherPlayer.transform.position = temp;
				SwapLine.GetComponent<EGA_Laser>().Swap();
				isSwapingPhase1 = false;
			}
		}
		else if (isSwapingPhase2)
		{

			_slicePlayer -= 20 * Time.deltaTime;
			_playerRenderer.material.SetFloat("_Slice", _slicePlayer);
			_otherPlayerRenderer.material.SetFloat("_Slice", _slicePlayer);
			if (_slicePlayer <= -3)
			{
				GetComponent<vThirdPersonInput>().enabled = true;
				GetComponent<vThirdPersonInput>().cc.enabled = true;
				_otherPlayer.GetComponent<vThirdPersonInput>().enabled = true;
				_otherPlayer.GetComponent<vThirdPersonInput>().cc.enabled = true;
				isSwapingPhase2 = false;
			}
		}
	}
}
