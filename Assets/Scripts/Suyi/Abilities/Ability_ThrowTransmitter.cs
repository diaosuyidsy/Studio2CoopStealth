using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
public class Ability_ThrowTransmitter : Ability
{
	public string SecondaryButtonName;
	public float ThrowThrust = 10f;
	[Range(10f, 90f)]
	public float ThrowAngle = 45f;
	public float FetchRadius = 1.5f;
	public Transform ThrowMark;
	public Transform TeleportTransmitter;
	public LayerMask ThrowMarkLandMask;
	public LayerMask ObstacleMask;

	private float firingAngle = 45f;
	private float ThrowMarkMoveSpeed = 5f;
	private float gravity = 25f;
	private LineRenderer lineRenderer;
	private float HRAxis;
	private float VRAxis;
	private float lineStepPerTime = 0.1f;
	private float lineMaxTime = 10f;
	private Vector3 _startVelocityCache;
	private Vector3 StartVelocity
	{
		get
		{
			if (Mathf.Approximately(HRAxis, 0f) && Mathf.Approximately(VRAxis, 0f))
			{
				Vector3 temp = new Vector3(_startVelocityCache.x, 0f, _startVelocityCache.z);
				float mgtemp = Mathf.Tan(ThrowAngle * Mathf.Deg2Rad) * temp.magnitude;
				temp.y = mgtemp;
				_startVelocityCache = temp.normalized * ThrowThrust;
				return _startVelocityCache;
			}
			Vector3 result = new Vector3(HRAxis, 0f, VRAxis);
			float mag = Mathf.Tan(ThrowAngle * Mathf.Deg2Rad) * result.magnitude;
			result.y = mag;
			_startVelocityCache = result.normalized * ThrowThrust;
			return _startVelocityCache;
		}
	}

	public override void Awake()
	{
		base.Awake();
		lineRenderer = GetComponent<LineRenderer>();
	}

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
		HRAxis = _player.GetAxis("Move Horizontal");
		VRAxis = _player.GetAxis("Move Vertical");
		float VLAxis = _player.GetAxis("Camera Move Vertical");
		ThrowAngle += VLAxis * ThrowMarkMoveSpeed;
		ThrowAngle = Mathf.Clamp(ThrowAngle, 10f, 90f);
		DrawTrajectory();

		transform.LookAt(new Vector3(ThrowMark.position.x, transform.position.y, ThrowMark.position.z));
	}

	/// <summary>
	/// If Player Lift Up LT, then reset everything
	/// </summary>
	public override void OnLiftUpAbility()
	{
		if (!ThrowMark.gameObject.activeSelf) return;
		EventManager.TriggerEvent($"Player{PlayerID}Free");
		lineRenderer.enabled = false;
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
		GameObject nearbyPlayer = _hasOtherPlayerNearby(FetchRadius);
		if (nearbyPlayer == null) return;
		nextReadyTime = Time.time + BaseCoolDown;
		coolDownTimeLeft = BaseCoolDown;
		//TeleportTransmitter.gameObject.SetActive(true);
		//TeleportTransmitter.parent = null;
		//TeleportTransmitter.position = transform.position + new Vector3(0f, 0f, 0.5f);
		nearbyPlayer.transform.position = transform.position + new Vector3(0f, 0f, 0.5f);
		nearbyPlayer.transform.rotation = transform.rotation;
		nearbyPlayer.GetComponent<Rigidbody>().velocity = StartVelocity;
		OnLiftUpAbility();
	}

	private GameObject _hasOtherPlayerNearby(float radius)
	{
		Collider[] hits = Physics.OverlapSphere(transform.position, radius, 1 << LayerMask.NameToLayer("Player"));
		foreach (var hit in hits)
		{
			if (!hit.CompareTag(tag)) return hit.gameObject;
		}
		return null;
	}

	private void DrawTrajectory()
	{
		var points = GetTrajectoryPoints(transform.position, StartVelocity, lineStepPerTime, lineMaxTime);
		if (lineRenderer)
		{
			if (!lineRenderer.enabled) lineRenderer.enabled = true;
			lineRenderer.positionCount = points.Count;
			lineRenderer.SetPositions(points.ToArray());
		}
		if (ThrowMark.gameObject)
		{
			if (!ThrowMark.gameObject.activeSelf) ThrowMark.gameObject.SetActive(true);
			if (points.Count > 1)
				ThrowMark.position = points[points.Count - 1];
		}
	}

	private List<Vector3> GetTrajectoryPoints(Vector3 start, Vector3 startVelocity, float timestep, float maxTime)
	{
		Vector3 prev = start;
		List<Vector3> points = new List<Vector3>();
		points.Add(prev);
		for (int i = 1; ; i++)
		{
			float t = timestep * i;
			if (t > maxTime) break;
			Vector3 pos = PlotTrajectoryAtTime(start, startVelocity, t);
			RaycastHit hit;
			if (Physics.Linecast(prev, pos, out hit, ObstacleMask))
			{
				points.Add(hit.point);
				break;
			}
			points.Add(pos);
			prev = pos;
		}
		return points;
	}

	private Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
	{
		return start + startVelocity * time + Physics.gravity * time * time * 0.5f;
	}
}

[CustomEditor(typeof(Ability_ThrowTransmitter))]
public class DrawWireArc : Editor
{
	private void OnSceneGUI()
	{
		Handles.color = Color.red;
		Ability_ThrowTransmitter AA = (Ability_ThrowTransmitter)target;
		Handles.DrawWireArc(AA.transform.position - new Vector3(0f, 1f, 0f), AA.transform.up, AA.transform.forward, 360f, Mathf.Pow(AA.ThrowThrust, 2f) / -Physics.gravity.y);
		Handles.DrawWireArc(AA.transform.position - new Vector3(0f, 1f, 0f), AA.transform.up, AA.transform.forward, 360f, AA.FetchRadius);
	}
}

//float HAxis = _player.GetAxis("Move Horizontal");
//float VAxis = _player.GetAxis("Move Vertical");
//Vector3 newPosition = ThrowMark.position + new Vector3(HAxis, 0f, VAxis) * ThrowMarkMoveSpeed;
//RaycastHit hit;
//if (Physics.Raycast(newPosition + new Vector3(0, 20f), Vector3.down, out hit, Mathf.Infinity, ThrowMarkLandMask))
//	newPosition.y = hit.point.y;
//Vector3 centerPosition = transform.position - new Vector3(0f, 1f);
//float distance = Vector3.Distance(newPosition, centerPosition);

//if (distance > Range)
//{
//	Vector3 fromOriginToObject = newPosition - centerPosition;
//	fromOriginToObject *= Range / distance;
//	newPosition = centerPosition + fromOriginToObject;
//	ThrowMark.position = newPosition;
//}
//else ThrowMark.position = newPosition;

//IEnumerator SimulateProjectile()
//{
//	TeleportTransmitter.gameObject.SetActive(true);
//	TeleportTransmitter.parent = null;
//	// Move projectile to the position of throwing object + add some offset if needed.
//	TeleportTransmitter.position = transform.position + new Vector3(0f, 0f, 0.5f);

//	// Calculate distance to target
//	float targetDistance = Vector3.Distance(TeleportTransmitter.position, ThrowMark.position);

//	// Calculate the velocity needed to throw the object to the target at specified angle.
//	float projectileVelocity = targetDistance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

//	// Extract the X  Y componenent of the velocity
//	float Vx = Mathf.Sqrt(projectileVelocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
//	float Vy = Mathf.Sqrt(projectileVelocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

//	// Calculate flight time.
//	float flightDuration = targetDistance / Vx;

//	TeleportTransmitter.rotation = Quaternion.LookRotation(ThrowMark.position - TeleportTransmitter.position);

//	OnLiftUpAbility();

//	float elapsedTime = 0f;

//	while (elapsedTime < flightDuration)
//	{
//		TeleportTransmitter.Translate(0, (Vy - (gravity * elapsedTime)) * Time.deltaTime, Vx * Time.deltaTime);
//		elapsedTime += Time.deltaTime;
//		yield return null;
//	}
//	TeleportTransmitter.rotation = Quaternion.identity;
//}
