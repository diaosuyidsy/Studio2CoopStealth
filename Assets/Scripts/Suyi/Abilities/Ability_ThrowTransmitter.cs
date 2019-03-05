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
	public Transform TeleportTransmitter;
	public LayerMask ThrowMarkLandMask;

	private float firingAngle = 45f;
	private float gravity = 25f;

	private void Update()
	{
		bool coolDownComplete = Time.time > nextReadyTime;
		if (coolDownComplete)
		{
			if (_player.GetButtonDown(ButtonName))
				OnPressedDownAbility();
			if (_player.GetButton(ButtonName))
			{
				OnPressingAbility();
				if (_player.GetButtonDown(SecondaryButtonName))
					OnPressedDownSecondaryAbility();
			}

			if (_player.GetButtonUp(ButtonName))
				OnLiftUpAbility();
		}
		else CoolDown();
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
		Vector3 newPosition = ThrowMark.position + new Vector3(HAxis, 0f, VAxis) * ThrowMarkMoveSpeed;
		RaycastHit hit;
		if (Physics.Raycast(newPosition + new Vector3(0, 20f), Vector3.down, out hit, Mathf.Infinity, ThrowMarkLandMask))
			newPosition.y = hit.point.y;
		Vector3 centerPosition = transform.position - new Vector3(0f, 1f);
		float distance = Vector3.Distance(newPosition, centerPosition);

		if (distance > Range)
		{
			Vector3 fromOriginToObject = newPosition - centerPosition;
			fromOriginToObject *= Range / distance;
			newPosition = centerPosition + fromOriginToObject;
			ThrowMark.position = newPosition;
		}
		else ThrowMark.position = newPosition;

		transform.LookAt(new Vector3(ThrowMark.position.x, transform.position.y, ThrowMark.position.z));
	}

	/// <summary>
	/// If Player Lift Up LT, then reset everything
	/// </summary>
	public override void OnLiftUpAbility()
	{
		if (!ThrowMark.gameObject.activeSelf) return;
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
		if (!ThrowMark.gameObject.activeSelf) return;
		nextReadyTime = Time.time + BaseCoolDown;
		coolDownTimeLeft = BaseCoolDown;
		StartCoroutine(SimulateProjectile());
	}

	IEnumerator SimulateProjectile()
	{
		TeleportTransmitter.gameObject.SetActive(true);
		TeleportTransmitter.parent = null;
		// Move projectile to the position of throwing object + add some offset if needed.
		TeleportTransmitter.position = transform.position + new Vector3(0f, 0f, 0.5f);

		// Calculate distance to target
		float targetDistance = Vector3.Distance(TeleportTransmitter.position, ThrowMark.position);

		// Calculate the velocity needed to throw the object to the target at specified angle.
		float projectileVelocity = targetDistance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

		// Extract the X  Y componenent of the velocity
		float Vx = Mathf.Sqrt(projectileVelocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
		float Vy = Mathf.Sqrt(projectileVelocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

		// Calculate flight time.
		float flightDuration = targetDistance / Vx;

		TeleportTransmitter.rotation = Quaternion.LookRotation(ThrowMark.position - TeleportTransmitter.position);

		OnLiftUpAbility();

		float elapsedTime = 0f;

		while (elapsedTime < flightDuration)
		{
			TeleportTransmitter.Translate(0, (Vy - (gravity * elapsedTime)) * Time.deltaTime, Vx * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		TeleportTransmitter.rotation = Quaternion.identity;
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
