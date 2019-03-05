using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Ability_ThrowTransmitter : Ability
{
	public string SecondaryButtonName;
	public float Range = 10f;
	public float ThrowMarkMoveSpeed = 5f;
	public Transform ThrowMark;

	private void Update()
	{
		if (_player.GetButtonDown(ButtonName))
		{
			OnPressedDownAbility();
		}
		if (_player.GetButton(ButtonName))
		{
			OnPressingAbility();
		}
		if (_player.GetButtonUp(ButtonName))
		{
			OnLiftUpAbility();
		}
	}

	/// <summary>
	/// On Pressed LT, set player into using ability status
	/// Detach ThrowMark so that player could lookat it
	/// Set ThrowMark Active
	/// </summary>
	public override void OnPressedDownAbility()
	{
		if (_isUsingOtherAbility) return;
		EventManager.TriggerEvent($"Player{PlayerID}InAbility");
		ThrowMark.parent = null;
		ThrowMark.gameObject.SetActive(true);
	}

	/// <summary>
	/// When Player is holding LT, Move ThrowMark using Joystick in a circle
	/// </summary>
	private void OnPressingAbility()
	{
		if (!ThrowMark.gameObject.activeSelf) return;
		float HAxis = _player.GetAxis("Move Horizontal");
		float VAxis = _player.GetAxis("Move Vertical");
		Vector3 targetPosition = ThrowMark.position + new Vector3(HAxis, 0f, VAxis);
		if (Vector3.Distance(targetPosition, transform.position - new Vector3(0f, 1f)) < Range)
		{
			ThrowMark.position = ThrowMark.position + new Vector3(HAxis, 0f, VAxis) * ThrowMarkMoveSpeed;
			transform.LookAt(new Vector3(ThrowMark.position.x, transform.position.y, ThrowMark.position.z));
		}
	}

	/// <summary>
	/// If Player Lift Up LT, then reset everything
	/// </summary>
	public override void OnLiftUpAbility()
	{
		EventManager.TriggerEvent($"Player{PlayerID}Free");
		ThrowMark.parent = transform;
		ThrowMark.localPosition = new Vector3(0, -1f, 0f);
		ThrowMark.gameObject.SetActive(false);
	}

	/// <summary>
	/// On Pressed RT
	/// </summary>
	private void OnPressedDownSecondaryAbility()
	{

	}
}

[CustomEditor(typeof(Ability_ThrowTransmitter))]
public class DrawWireArc : Editor
{
	private void OnSceneGUI()
	{
		Handles.color = Color.red;
		Ability_ThrowTransmitter AA = (Ability_ThrowTransmitter)target;
		Handles.DrawWireArc(AA.transform.position - new Vector3(0f, 1f, 0f), AA.transform.up, AA.transform.forward, 360f, AA.Range);
	}
}
