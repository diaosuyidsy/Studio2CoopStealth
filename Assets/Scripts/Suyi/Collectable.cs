using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rewired;

public class Collectable : MonoBehaviour
{
	public enum Abilities
	{
		Swap,
		Teleport,
		Assasin,
		ThrowTransmitter,
	}
	public Abilities _unlockAbility;
	[Tooltip("Is it used for energy regen?")]
	public bool IsEnergyRegen = false;
	[Tooltip("Which Player Can Collect this thing, 1 for BigPlayer, 2 for SmallPlayer, 0 for none, 3 for both")]
	public int PlayerID = 1;
	public string ActionButton = "X";
	public bool AutoTriggerAction = false;
	public bool DestroyAfter = false;
	public float DestroyDelay = 0f;
	public float OnDoActionDelay = 0f;
	public UnityEvent OnDoAction;
	public UnityEvent OnPlayerEnter;
	public UnityEvent OnPlayerStay;
	public UnityEvent OnPlayerExit;

	private GameObject _player1;
	private GameObject _player2;
	private string _allowedPlayerTag;
	private bool _rightPlayerEntered;

	private void Awake()
	{
		_player1 = GameObject.FindGameObjectWithTag("Player1");
		_player2 = GameObject.FindGameObjectWithTag("Player2");
		Debug.Assert(_player1 != null);
		Debug.Assert(_player2 != null);
		_allowedPlayerTag = "Player";
		if (PlayerID != 3) _allowedPlayerTag += PlayerID.ToString();
	}

	#region CollectEffect
	public void UnlockAbility()
	{
		switch (_unlockAbility)
		{
			case Abilities.Assasin:
				_player1.GetComponent<Ability_Assasination>().enabled = true;
				break;

			case Abilities.Swap:
				_player2.GetComponent<Ability_Swap>().enabled = true;
				_player2.GetComponent<Ability_Swap>().SwapLine.SetActive(true);
				break;

			case Abilities.Teleport:
				_player2.GetComponent<Ability_Teleport>().enabled = true;
				break;
			case Abilities.ThrowTransmitter:
				_player1.GetComponent<Ability_ThrowTransmitter>().enabled = true;
				break;
		}
	}

	public void RegenEnergy(int _eng)
	{
		EnergyManager.instance.CanUseEnergy(-_eng);
	}
	#endregion

	IEnumerator _OnDoActionWithDelay(float time)
	{
		yield return new WaitForSeconds(time);
		OnDoAction.Invoke();
		if (DestroyAfter)
			Destroy(gameObject, DestroyDelay);
	}

	private void Update()
	{
		if (_rightPlayerEntered)
		{
			switch (PlayerID)
			{
				case 1:
				case 2:
					if (ReInput.players.GetPlayer(PlayerID - 1).GetButtonDown(ActionButton))
					{
						if (IsEnergyRegen && EnergyManager.instance.MaxEnergy == EnergyManager.instance._currentEnergy) return;
						StartCoroutine(_OnDoActionWithDelay(OnDoActionDelay));
					}
					break;
				case 3:
					if (ReInput.players.GetPlayer(0).GetButtonDown(ActionButton) || ReInput.players.GetPlayer(1).GetButtonDown(ActionButton))
					{
						if (IsEnergyRegen && EnergyManager.instance.MaxEnergy == EnergyManager.instance._currentEnergy) return;
						StartCoroutine(_OnDoActionWithDelay(OnDoActionDelay));
					}
					break;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag.Contains(_allowedPlayerTag))
		{
			_rightPlayerEntered = true;
			OnPlayerEnter.Invoke();
			if (AutoTriggerAction)
			{
				if (IsEnergyRegen && EnergyManager.instance.MaxEnergy == EnergyManager.instance._currentEnergy) return;
				StartCoroutine(_OnDoActionWithDelay(OnDoActionDelay));
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.tag.Contains(_allowedPlayerTag))
		{
			OnPlayerStay.Invoke();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag.Contains(_allowedPlayerTag))
		{
			_rightPlayerEntered = false;
			OnPlayerExit.Invoke();
		}
	}
}
