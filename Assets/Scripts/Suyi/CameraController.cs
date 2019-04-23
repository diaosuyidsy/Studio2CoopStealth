using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float SmoothSpeed = 0.04f;

	private Vector3 _desiredPosition;
	private Vector3 _desiredRotation;
	private Transform _desiredRoomCenter;
	private float _wiggleX = 2f;
	private float _wiggleZ = 2f;

	private GameObject[] _players;
	private Vector3 _followTarget;
	private enum CState
	{
		transitioning,
		cStatic,
		dynamic,
	}
	private CState _cameraState = CState.cStatic;

	private void Awake()
	{
		_players = new GameObject[2];
		_players[0] = GameObject.FindGameObjectWithTag("Player1");
		_players[1] = GameObject.FindGameObjectWithTag("Player2");
	}

	public void SetCameraPosRot(Vector3 _pos, Vector3 _rot, Transform _rc, float _time)
	{
		_desiredRotation = _rot;
		_desiredPosition = _pos;
		_desiredRoomCenter = _rc;
		StartCoroutine(transition(_time));
	}

	public void SetCameraPosRot(Transform _tran, Transform _rc, float _time)
	{
		SetCameraPosRot(_tran.position, _tran.eulerAngles, _rc, _time);
	}

	private void Update()
	{
		if (_cameraState != CState.dynamic) return;
		_setTarget();
		var xdiff = _followTarget.x - _desiredRoomCenter.position.x;
		var zdiff = _followTarget.z - _desiredRoomCenter.position.z;
		var _dPosition = new Vector3(_desiredPosition.x + xdiff, _desiredPosition.y, _desiredPosition.z + zdiff);
		var _smoothedPosition = Vector3.Lerp(transform.position, _dPosition, SmoothSpeed);
		var _clampedPosition = new Vector3(Mathf.Clamp(_smoothedPosition.x, _desiredPosition.x - _wiggleX, _desiredPosition.x + _wiggleX),
			_smoothedPosition.y,
			Mathf.Clamp(_smoothedPosition.z, _desiredPosition.z - _wiggleZ, _desiredPosition.z + _wiggleZ)
			);
		transform.position = _clampedPosition;
	}

	/// <summary>
	/// Transition the camera to desired position/rotation
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	IEnumerator transition(float time)
	{
		_cameraState = CState.transitioning;
		// First calculate the actual position of camera
		_setTarget();
		var xdiff = _followTarget.x - _desiredRoomCenter.position.x;
		var zdiff = _followTarget.z - _desiredRoomCenter.position.z;
		var _dPosition = new Vector3(_desiredPosition.x + xdiff, _desiredPosition.y, _desiredPosition.z + zdiff);
		var _clampedPosition = new Vector3(Mathf.Clamp(_dPosition.x, _desiredPosition.x - _wiggleX, _desiredPosition.x + _wiggleX),
			_dPosition.y,
			Mathf.Clamp(_dPosition.z, _desiredPosition.z - _wiggleZ, _desiredPosition.z + _wiggleZ)
			);
		// End calculation
		float elapsedTime = 0f;
		Vector3 initialPosition = transform.position;
		Vector3 initialEularRotation = transform.rotation.eulerAngles;
		while (elapsedTime < time)
		{
			transform.position = Vector3.Lerp(initialPosition, _clampedPosition, elapsedTime / time);
			transform.eulerAngles = new Vector3(
				Mathf.Lerp(initialEularRotation.x, _desiredRotation.x, elapsedTime / time),
				Mathf.Lerp(initialEularRotation.y, _desiredRotation.y, elapsedTime / time),
				Mathf.Lerp(initialEularRotation.z, _desiredRotation.z, elapsedTime / time)
				);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		_cameraState = CState.dynamic;
	}

	private void _setTarget()
	{
		// Need to set Follow Target to be average of all players
		Vector3 total = Vector3.zero;
		int length = 0;

		foreach (GameObject go in _players)
		{
			if (go != null && go.activeSelf)
			{
				total += go.transform.position;
				length++;
			}
		}
		total /= (length == 0 ? 1 : length);
		_followTarget = total;
	}

}

//[HideInInspector]
//public Vector3 FollowTarget;

//public float SmoothSpeed = 0.04f;

//public float FOVSizeMin = 8f;
//public float FOVSizeMax = 35f;
//public float XOffset = 0f;
//public float ZOffset = -0.33f;
//public float WonFOVSize = 6f;
//public float AdjustHeight = 0f;

//private float _startPosY;
//private float _maxDistanceOrigin;
//private float _xDiffOrigin;
//private float _zDiffOrigin;
//private Vector3 _desiredPosition;
//private Vector3 _smoothedPosition;
//private float _desiredFOV;
//private float _smoothedFOV;
//private bool _winLock = false;
//private GameObject[] Players;

//// Use this for initialization
//void Start()
//{
//	Players = new GameObject[2];
//	Players[0] = GameObject.FindGameObjectWithTag("Player1");
//	Players[1] = GameObject.FindGameObjectWithTag("Player2");
//	SetTarget(false);
//	// Set the max Distance originally
//	float maxDist = 0f;
//	_maxDistanceOrigin = maxDist;
//	_startPosY = transform.position.y;
//}

//// Update is called once per frame
//void Update()
//{
//	// If player won, then do Won Logic and lock others
//	if (_winLock)
//	{
//		_desiredPosition = new Vector3(FollowTarget.x + _xDiffOrigin, transform.position.y, FollowTarget.z + _zDiffOrigin);
//		_smoothedPosition = Vector3.Lerp(transform.position, _desiredPosition, SmoothSpeed);
//		transform.position = _smoothedPosition;
//		GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, WonFOVSize, SmoothSpeed);
//		return;
//	}
//	SetTarget();
//	if (FollowTarget == Vector3.zero || FollowTarget == new Vector3(XOffset, 0f, ZOffset))
//	{
//		return;
//	}
//	_desiredPosition = new Vector3(FollowTarget.x + _xDiffOrigin, _startPosY, FollowTarget.z + _zDiffOrigin) - AdjustHeight * transform.forward.normalized;
//	_smoothedPosition = Vector3.Lerp(transform.position, _desiredPosition, SmoothSpeed);
//	transform.position = _smoothedPosition;
//	_desiredFOV = 2f * MaxDistance() + 3.99f;
//	GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, _desiredFOV, SmoothSpeed);
//	GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView, FOVSizeMin, FOVSizeMax);
//}

//public void OnWinCameraZoom(Transform tar)
//{
//	_winLock = true;
//	FollowTarget = tar.position;
//}

//void SetTarget(bool withOffset = true)
//{
//	// Need to set Follow Target to be average of all players
//	Vector3 total = Vector3.zero;
//	int length = 0;

//	foreach (GameObject go in Players)
//	{
//		if (go != null && go.activeSelf)
//		{
//			total += go.transform.position;
//			length++;
//		}
//	}
//	total /= (length == 0 ? 1 : length);
//	if (withOffset)
//	{
//		total.x += XOffset;
//		total.z += ZOffset;
//	}
//	FollowTarget = total;
//}

//// Should get the max distance between any two players
//float MaxDistance()
//{
//	float maxDist = 0f;
//	maxDist = Vector3.Distance(Players[0].transform.position, Players[1].transform.position);
//	return maxDist;
//}