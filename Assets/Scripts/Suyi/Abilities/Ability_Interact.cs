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
	private Interactable _temp;

	public override void Awake()
	{
		base.Awake();
		_coll = GetComponent<Collider>();
		Debug.Assert(_coll != null);
	}

	private void Update()
	{
		if (_player.GetButtonDown(ButtonName)) OnPressedDownAbility();
		if (_player.GetButton(ButtonName)) OnHoldAbility();
		if (_player.GetButtonUp(ButtonName)) OnLiftUpAbility();
	}

	/// <summary>
	/// Scans for nearby (in range) interactable object and assign then to _int
	/// </summary>
	/// <param name="_int"></param>
	/// <returns>true if there is interactablel object nearby</returns>
	private bool _scanInteractable(out Interactable _int)
	{
		_int = null;
		Collider[] hitcolliders = Physics.OverlapSphere(_coll.bounds.center, Range, InteractableMask);

		if (hitcolliders.Length <= 0) return false;

		float minDist = Mathf.Infinity;
		GameObject smallestGO = null;
		for (int i = 0; i < hitcolliders.Length; i++)
		{
			var hit = hitcolliders[i].gameObject;
			print(hit.name);
			var dist = Vector3.Distance(hit.transform.position, _coll.bounds.center);
			//if (dist < minDist && hit.GetComponent<Interactable>() && !Physics.Linecast(transform.position, hit.transform.position, 1 << LayerMask.NameToLayer("Default")))
			if (dist < minDist && hit.GetComponent<Interactable>())
			{
				smallestGO = hit.gameObject;
				minDist = dist;
			}
		}
		print(smallestGO.name);
		if (!smallestGO) return false;
		_int = smallestGO.GetComponent<Interactable>();
		return true;

	}

	public override void OnPressedDownAbility()
	{
		if (_scanInteractable(out _interactable) && !_interactable.IsOccupied)
		{
			_interactable.OnInteractDown(gameObject);
			_isInteracting = true;
			EventManager.TriggerEvent($"Player{PlayerID}InAbility");
		}
	}

	public override void OnHoldAbility()
	{
		if (!_isInteracting) return;
		if (!_scanInteractable(out _temp))
		{
			OnLiftUpAbility();
			return;
		}

		_interactable.OnInteracting(gameObject);
	}

	public override void OnLiftUpAbility()
	{
		if (!_isInteracting || !_interactable) return;
		EventManager.TriggerEvent($"Player{PlayerID}Free");
		_interactable.OnInteractUp(gameObject);
		_isInteracting = false;
		_interactable = null;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		EventManager.StartListening("OnRewind", () =>
		{
			OnLiftUpAbility();
		});
	}

	public override void OnDisable()
	{
		base.OnDisable();
		EventManager.StopListening("OnRewind", () =>
		{
			OnLiftUpAbility();
		});
	}
}
