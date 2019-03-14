using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Ability_Interact : Ability
{
	public float Range = 2f;

	private Interactable _interactable;
	[SerializeField] private LayerMask InteractableMask;
	private Collider _coll;
	private bool _isInteracting;

	public override void Awake()
	{
		base.Awake();
		_coll = GetComponent<Collider>();
		Debug.Assert(_coll != null);
	}

	private void Update()
	{
		if (_scanInteractable(out _interactable))
		{
			if (_player.GetButtonDown(ButtonName)) OnPressedDownAbility();
			if (_player.GetButton(ButtonName)) OnHoldAbility();
			if (_player.GetButtonUp(ButtonName)) OnLiftUpAbility();
		}
	}

	/// <summary>
	/// Scans for nearby (in range) interactable object and assign then to _int
	/// </summary>
	/// <param name="_int"></param>
	/// <returns>true if there is interactablel object nearby</returns>
	private bool _scanInteractable(out Interactable _int)
	{
		Collider[] hitcolliders = Physics.OverlapSphere(_coll.bounds.center, Range, InteractableMask);

		if (hitcolliders.Length <= 0)
		{
			_int = null;
			return false;
		}

		float minDist = Mathf.Infinity;
		GameObject smallestGO = null;
		for (int i = 0; i < hitcolliders.Length; i++)
		{
			var hit = hitcolliders[i].gameObject;
			var dist = Vector3.Distance(hit.transform.position, _coll.bounds.center);
			if (dist < minDist)
			{
				smallestGO = hit.gameObject;
				minDist = dist;
			}
		}
		_int = smallestGO.GetComponent<Interactable>();
		return true;

	}

	public override void OnPressedDownAbility()
	{
		if (!_interactable.IsOccupied)
		{
			_interactable.OnInteractDown();
			_isInteracting = true;
			EventManager.TriggerEvent($"Player{PlayerID}InAbility");
		}
	}

	public override void OnHoldAbility()
	{
		if (!_isInteracting) return;
		_interactable.OnInteracting();
	}

	public override void OnLiftUpAbility()
	{
		if (!_isInteracting) return;
		EventManager.TriggerEvent($"Player{PlayerID}Free");
		_interactable.OnInteractUp();
		_isInteracting = false;
	}
}
