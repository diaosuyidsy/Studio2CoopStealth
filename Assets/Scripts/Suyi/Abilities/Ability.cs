using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using Invector.vItemManager;
using Invector.vShooter;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(vShooterMeleeInput))]
[RequireComponent(typeof(vThirdPersonController))]
public abstract class Ability : MonoBehaviour
{
	public string ButtonName;
	public float BaseCoolDown = 1f;
	public int EnergyCost = 1;
	public string UIFillName;

	protected float coolDownTimeLeft = 0f;
	protected float nextReadyTime;
	protected Player _player;
	protected bool _isUsingOtherAbility;
	protected int PlayerID;
	protected Image UIFill;

	public virtual void Awake()
	{
		PlayerID = GetComponent<vShooterMeleeInput>().playerId;
		_player = ReInput.players.GetPlayer(PlayerID);
		GameObject temp = GameObject.Find(UIFillName);
		if (temp != null)
			UIFill = temp.GetComponent<Image>();
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
		float percentage = coolDownTimeLeft / BaseCoolDown;
		if (UIFill != null)
		{
			UIFill.fillAmount = 1 - percentage;
			if (coolDownTimeLeft < 0.1f)
			{
				UIFill.fillAmount = 1f;
				UIFill.transform.parent.parent.GetComponent<DOTweenAnimation>().DOPlayById("OnCoolDown");
				nextReadyTime = Time.time - 0.1f;
			}
		}
	}

	/// <summary>
	/// On Use Ability, Spend Energy
	/// </summary>
	public virtual bool SpendEnergy()
	{
		return EnergyManager.instance.CanUseEnergy(EnergyCost);
	}

	public virtual void OnEnable()
	{
		EventManager.StartListening($"Player{PlayerID}InAbility",
			() =>
			{
				GetComponent<vThirdPersonController>().enabled = false;
				GetComponent<vShooterMeleeInput>().enabled = false;
				_isUsingOtherAbility = true;
			}
		);
		EventManager.StartListening($"Player{PlayerID}Free",
			() =>
			{
				GetComponent<vThirdPersonController>().enabled = true;
				GetComponent<vShooterMeleeInput>().enabled = true;
				_isUsingOtherAbility = false;
			}
		);
		EventManager.StartListening("OnRewind", () =>
		{
			EventManager.TriggerEvent($"Player{PlayerID}Free");
		});
	}

	public virtual void OnDisable()
	{
		EventManager.StopListening($"Player{PlayerID}InAbility",
			() =>
			{
				GetComponent<vThirdPersonController>().enabled = false;
				GetComponent<vShooterMeleeInput>().enabled = false;
				_isUsingOtherAbility = true;
			}
		);
		EventManager.StopListening($"Player{PlayerID}Free",
			() =>
			{
				GetComponent<vThirdPersonController>().enabled = true;
				GetComponent<vShooterMeleeInput>().enabled = true;
				_isUsingOtherAbility = false;
			}
		);
		EventManager.StopListening("OnRewind", () =>
		{
			EventManager.TriggerEvent($"Player{PlayerID}Free");
		});
	}
}
