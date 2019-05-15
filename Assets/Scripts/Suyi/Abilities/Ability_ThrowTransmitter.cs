using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
public class Ability_ThrowTransmitter : Ability
{
	public string SecondaryButtonName;
	public float ThrowThrust = 10f;
	public float FetchRadius = 1.5f;
	public float ThrowMarkMoveSpeed = 0.5f;
	public float ThrowMarkGravityScale = 1f;
	public Transform ThrowMark;
	public Transform ShadowThrowMark;
	public Transform TeleportTransmitter;
	public LayerMask ThrowMarkLandMask;
	public LayerMask ObstacleMask;

	[Range(10f, 89f)]
	private float ThrowAngle = 45f;
	private float firingAngle = 45f;
	private Vector3 _throwMarkGravity
	{
		get
		{
			return Physics.gravity * ThrowMarkGravityScale;
		}
	}
	private LineRenderer lineRenderer;
	private float HRAxis;
	private float VRAxis;
	private float lineStepPerTime = 0.1f;
	private float lineMaxTime = 10f;
	private Vector3 _startVelocityCache;
	private Vector3 _bodyCenter
	{
		get
		{
			var coll = GetComponent<Collider>();
			Debug.Assert(coll != null);
			var pos = transform.position;
			pos.y += coll.bounds.extents.y;
			return pos;
		}
	}
	//private Vector3 StartVelocity
	//{
	//	get
	//	{
	//		if (Mathf.Approximately(HRAxis, 0f) && Mathf.Approximately(VRAxis, 0f))
	//		{
	//			Vector3 temp = new Vector3(_startVelocityCache.x, 0f, _startVelocityCache.z);
	//			float mgtemp = Mathf.Tan(ThrowAngle * Mathf.Deg2Rad) * temp.magnitude;
	//			temp.y = mgtemp;
	//			_startVelocityCache = temp.normalized * ThrowThrust;
	//			return _startVelocityCache;
	//		}
	//		Vector3 tp = new Vector3(HRAxis, 0f, VRAxis);
	//		var noy = new Vector3(_startVelocityCache.x, 0f, _startVelocityCache.z);
	//		Vector3 result = Vector3.Slerp(noy, tp, Time.deltaTime * 2f);
	//		float mag = Mathf.Tan(ThrowAngle * Mathf.Deg2Rad) * result.magnitude;
	//		result.y = mag;
	//		_startVelocityCache = result.normalized * ThrowThrust;
	//		return _startVelocityCache;
	//	}
	//}
	private Vector3 StartVelocity
	{
		get
		{
			float diffz = ShadowThrowMark.position.z - transform.position.z;
			float diffx = ShadowThrowMark.position.x - transform.position.x;
			Vector3 result = new Vector3(diffx, 0f, diffz);
			float mag = Mathf.Tan(ThrowAngle * Mathf.Deg2Rad) * result.magnitude;
			result.y = mag;
			_startVelocityCache = result.normalized * ThrowThrust;
			return _startVelocityCache;
		}
	}

	private float Range
	{
		get
		{
			return Mathf.Pow(ThrowThrust, 2) / -_throwMarkGravity.y - 1f;
		}
	}

	public override void Awake()
	{
		base.Awake();
		lineRenderer = GetComponent<LineRenderer>();
		_startVelocityCache = new Vector3(0, 1f, 1f);
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

	private void FixedUpdate()
	{
		TeleportTransmitter.GetComponent<Rigidbody>().AddForce(Vector3.down * Physics.gravity.y * (1f - ThrowMarkGravityScale));
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
		var rb = GetComponent<Rigidbody>();
		if (rb) rb.velocity = Vector3.zero;
		ThrowMark.parent = null;
		ThrowMark.gameObject.SetActive(true);

		ShadowThrowMark.parent = null;
		ShadowThrowMark.gameObject.SetActive(true);
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

		// Move the Shadow Mark
		Vector3 newPosition = ShadowThrowMark.position + new Vector3(HRAxis, 0f, VRAxis) * Time.deltaTime * ThrowMarkMoveSpeed;
		RaycastHit hit;
		if (Physics.Raycast(newPosition + new Vector3(0, 20f), Vector3.down, out hit, 100f, ThrowMarkLandMask))
			newPosition.y = hit.point.y;
		Vector3 centerPosition = _bodyCenter;
		float distance = Vector3.Distance(new Vector3(newPosition.x, 0f, newPosition.z), new Vector3(centerPosition.x, 0f, centerPosition.z));

		if (distance > Range)
		{
			Vector3 fromOriginToObject = newPosition - centerPosition;
			fromOriginToObject *= Range / distance;
			newPosition = centerPosition + fromOriginToObject;
			ShadowThrowMark.position = newPosition;
		}
		else ShadowThrowMark.position = newPosition;
		ThrowAngle = 90f - Mathf.Asin(-_throwMarkGravity.y * distance / (ThrowThrust * ThrowThrust)) * Mathf.Rad2Deg / 2f;
		// End Move

		//ThrowAngle += VLAxis * ThrowMarkMoveSpeed * Time.deltaTime;
		//ThrowAngle = Mathf.Clamp(ThrowAngle, 10f, 89f);
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
		nextReadyTime = Time.time + BaseCoolDown;
		coolDownTimeLeft = BaseCoolDown;
		TeleportTransmitter.gameObject.SetActive(true);
		TeleportTransmitter.parent = null;
		TeleportTransmitter.position = _bodyCenter;
		TeleportTransmitter.GetComponent<Rigidbody>().velocity = StartVelocity;
		EventManager.TriggerEvent($"Player{PlayerID}InAbility");
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
		var points = GetTrajectoryPoints(_bodyCenter, StartVelocity, lineStepPerTime, lineMaxTime);
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
		return start + startVelocity * time + _throwMarkGravity * time * time * 0.5f;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		EventManager.StartListening($"Player{PlayerID}InAbility",
			() =>
			{
				GetComponent<Animator>().SetFloat("InputVertical", 0f);
				GetComponent<Animator>().SetFloat("InputHorizontal", 0f);
				GetComponent<Animator>().SetFloat("InputMagnitude", 0f);
			}
		);
	}

	public override void OnDisable()
	{
		base.OnDisable();
		OnLiftUpAbility();
		EventManager.StopListening($"Player{PlayerID}InAbility",
			() =>
			{
				GetComponent<Animator>().SetFloat("InputVertical", 0f);
				GetComponent<Animator>().SetFloat("InputHorizontal", 0f);
				GetComponent<Animator>().SetFloat("InputMagnitude", 0f);
			}
		);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Ability_ThrowTransmitter))]
public class DrawWireArc : Editor
{
	private void OnSceneGUI()
	{
		Handles.color = Color.red;
		Ability_ThrowTransmitter AA = (Ability_ThrowTransmitter)target;
		Handles.DrawWireArc(AA.transform.position - new Vector3(0f, 1f, 0f), AA.transform.up, AA.transform.forward, 360f, Mathf.Pow(AA.ThrowThrust, 2f) / -Physics.gravity.y * AA.ThrowMarkGravityScale);
		Handles.DrawWireArc(AA.transform.position - new Vector3(0f, 1f, 0f), AA.transform.up, AA.transform.forward, 360f, AA.FetchRadius);
	}
}
#endif