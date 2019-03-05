using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(PlayerController_SS))]
public abstract class Ability : MonoBehaviour
{
	public string ButtonName;
	public float BaseCoolDown = 1f;

	protected float coolDownTimeLeft = 0f;
	protected float nextReadyTime;
	protected Player _player;
	protected bool _isUsingOtherAbility;
	protected int PlayerID;

	public virtual void Awake()
	{
		PlayerID = GetComponent<PlayerController_SS>().PlayerID;
		_player = ReInput.players.GetPlayer(PlayerID);
	}
	/// <summary>
	/// Pressed Down the Button
	/// </summary>
	public abstract void OnPressedDownAbility();

	/// <summary>
	/// Holding the button
	/// </summary>
	public virtual void OnHoldAbility() { }

	/// <summary>
	/// Lift Up the Button
	/// </summary>
	public virtual void OnLiftUpAbility() { }

	/// <summary>
	/// Cool Down Should run in Update
	/// </summary>
	public virtual void CoolDown()
	{
		coolDownTimeLeft -= Time.deltaTime;
	}

	public virtual void OnEnable()
	{
		EventManager.StartListening($"Player{PlayerID}InAbility", () => _isUsingOtherAbility = true);
		EventManager.StartListening($"Player{PlayerID}Free", () => _isUsingOtherAbility = false);
	}

	public virtual void OnDisable()
	{
		EventManager.StopListening($"Player{PlayerID}InAbility", () => _isUsingOtherAbility = true);
		EventManager.StopListening($"Player{PlayerID}Free", () => _isUsingOtherAbility = false);
	}



}
