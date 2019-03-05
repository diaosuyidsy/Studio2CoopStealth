using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Ability_Ultimate : Ability
{
	[Header("BaseCoolDown is used as charge, meaning MaxCharge")]
	public float Range = 5f;
	private GameObject Mech;
	private GameObject otherPlayer;
	[SerializeField] private float _curCharge;

	public override void Awake()
	{
		base.Awake();
		Mech = GameObject.FindGameObjectWithTag("Mech");
		int otherplayertagnum = PlayerID == 0 ? 2 : 1;
		otherPlayer = GameObject.FindGameObjectWithTag($"Player{otherplayertagnum}");
	}

	private void Update()
	{
		float distance = Vector3.Distance(transform.position, otherPlayer.transform.position);
		if (distance > Range) return;
		if (Mathf.Approximately(_curCharge, BaseCoolDown))
		{
			if (_player.GetButton(ButtonName))
			{
				OnPressedDownAbility();
			}
		}
	}

	public override void OnPressedDownAbility()
	{
		_curCharge = 0f;
		EventManager.TriggerEvent("CallUlt");
	}

	IEnumerator clearUlt(float time)
	{
		yield return new WaitForSeconds(time);
		_curCharge = 0f;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		EventManager.StartListening("CallUlt", () => StartCoroutine(clearUlt(Mech.GetComponent<UltController>().Tolerance)));
		EventManager.StartListening("ChargeUlt", () =>
		{
			if (_curCharge < BaseCoolDown) _curCharge += 0.25f;
		});
	}

	public override void OnDisable()
	{
		base.OnDisable();
		EventManager.StopListening("CallUlt", () => StartCoroutine(clearUlt(Mech.GetComponent<UltController>().Tolerance)));
		EventManager.StopListening("ChargeUlt", () =>
		{
			if (_curCharge < BaseCoolDown) _curCharge += 0.25f;
		});
	}
}

[CustomEditor(typeof(Ability_Ultimate))]
public class DrawUltimateWireArc : Editor
{
	private void OnSceneGUI()
	{
		Handles.color = Color.blue;
		Ability_Ultimate AA = (Ability_Ultimate)target;
		Handles.DrawWireArc(AA.transform.position - new Vector3(0f, 1f, 0f), AA.transform.up, AA.transform.forward, 360f, AA.Range);
	}
}
