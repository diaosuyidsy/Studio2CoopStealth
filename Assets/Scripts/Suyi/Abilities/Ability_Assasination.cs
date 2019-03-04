using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEditor;

public class Ability_Assasination : Ability
{
	public float Range = 1.5f;
	public float Angle = 60f;

	[SerializeField] private LayerMask EnemyMask;
	private GameObject _interactableObject;
	// Should not be modified since only player 0 could have this ability
	private readonly int PlayerID = 0;
	private Player _player;

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(PlayerID);
	}

	private void Update()
	{
		// First Get the nearby enemy, decide which one to interact
		_getNearbyEnemy();
		// And then see if stealth kill cd is complete, if so then can kill
		bool coolDownComplete = Time.time > nextReadyTime;
		if (_player.GetButtonDown(ButtonName))
		{
			if (coolDownComplete)
				OnPressedDownAbility();
			else CoolDown();
		}
	}

	public override void OnPressedDownAbility()
	{
		nextReadyTime = BaseCoolDown + Time.time;
		coolDownTimeLeft = BaseCoolDown;
		if (_interactableObject != null) _interactableObject.SetActive(false);
	}

	/// <summary>
	/// Get nearby enemy that is facing the same direction as player and return it into _interactableObject
	/// </summary>
	private void _getNearbyEnemy()
	{
		// First check if there is a enemy to kill
		Collider[] hitcolliders = Physics.OverlapSphere(transform.position, Range, EnemyMask);
		float smallestAngle = Mathf.Infinity;
		GameObject smallestGO = null;
		for (int i = 0; i < hitcolliders.Length; i++)
		{
			var hit = hitcolliders[i].gameObject;
			float angle = _angleBetween(hit);
			if (angle < Angle && angle < smallestAngle)
			{
				smallestGO = hit;
			}
		}
		_interactableObject = smallestGO;

	}

	private float _angleBetween(GameObject go)
	{
		Vector3 targetDir = go.transform.position - transform.position;
		float angle = Vector3.Angle(targetDir, transform.forward);
		return angle;
	}
}

[CustomEditor(typeof(Ability_Assasination))]
public class DrawSolidArc : Editor
{
	private void OnSceneGUI()
	{
		Handles.color = Color.yellow;
		Ability_Assasination AA = (Ability_Assasination)target;
		Handles.DrawSolidArc(AA.transform.position, AA.transform.up, AA.transform.forward, AA.Angle, AA.Range);
		Handles.DrawSolidArc(AA.transform.position, AA.transform.up, AA.transform.forward, -AA.Angle, AA.Range);
	}
}
