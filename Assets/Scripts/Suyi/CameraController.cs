using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[HideInInspector]
	public Vector3 FollowTarget;

	public float SmoothSpeed = 0.04f;

	public float FOVSizeMin = 8f;
	public float FOVSizeMax = 35f;
	public float XOffset = 0f;
	public float ZOffset = -0.33f;
	public float WonFOVSize = 6f;

	private float _maxDistanceOrigin;
	private float _xDiffOrigin;
	private float _zDiffOrigin;
	private Vector3 _desiredPosition;
	private Vector3 _smoothedPosition;
	private float _desiredFOV;
	private float _smoothedFOV;
	private bool _winLock = false;
	private GameObject[] Players;

	// Use this for initialization
	void Start()
	{
		Players = new GameObject[2];
		Players[0] = GameObject.FindGameObjectWithTag("Player1");
		Players[1] = GameObject.FindGameObjectWithTag("Player2");
		SetTarget(false);
		// Set the max Distance originally
		float maxDist = 0f;
		_maxDistanceOrigin = maxDist;
	}

	// Update is called once per frame
	void Update()
	{
		// If player won, then do Won Logic and lock others
		if (_winLock)
		{
			_desiredPosition = new Vector3(FollowTarget.x + _xDiffOrigin, transform.position.y, FollowTarget.z + _zDiffOrigin);
			_smoothedPosition = Vector3.Lerp(transform.position, _desiredPosition, SmoothSpeed);
			transform.position = _smoothedPosition;
			GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, WonFOVSize, SmoothSpeed);
			return;
		}
		SetTarget();
		if (FollowTarget == Vector3.zero || FollowTarget == new Vector3(XOffset, 0f, ZOffset))
		{
			return;
		}
		_desiredPosition = new Vector3(FollowTarget.x + _xDiffOrigin, transform.position.y, FollowTarget.z + _zDiffOrigin);
		_smoothedPosition = Vector3.Lerp(transform.position, _desiredPosition, SmoothSpeed);
		transform.position = _smoothedPosition;
		_desiredFOV = 2f * MaxDistance() + 3.99f;
		GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, _desiredFOV, SmoothSpeed);
		GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView, FOVSizeMin, FOVSizeMax);
	}

	public void OnWinCameraZoom(Transform tar)
	{
		_winLock = true;
		FollowTarget = tar.position;
	}

	void SetTarget(bool withOffset = true)
	{
		// Need to set Follow Target to be average of all players
		Vector3 total = Vector3.zero;
		int length = 0;

		foreach (GameObject go in Players)
		{
			if (go != null && go.activeSelf)
			{
				total += go.transform.position;
				length++;
			}
		}
		total /= (length == 0 ? 1 : length);
		if (withOffset)
		{
			total.x += XOffset;
			total.z += ZOffset;
		}
		FollowTarget = total;
	}

	// Should get the max distance between any two players
	float MaxDistance()
	{
		float maxDist = 0f;
		maxDist = Vector3.Distance(Players[0].transform.position, Players[1].transform.position);
		return maxDist;
	}
}
