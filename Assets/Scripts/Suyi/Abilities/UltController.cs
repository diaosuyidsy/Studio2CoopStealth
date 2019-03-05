using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class UltController : MonoBehaviour
{
	public float Tolerance = 1f;
	public float MaxMoveSpeed = 2f;
	public float MaxLookAtRotationDelta = 2f;
	public GameObject LookTarget;

	// Can the player control the Mech?
	private bool _canControl;
	// Is the Mech called by a player?
	private bool _called;
	private float _timeFirstCalled;
	private GameObject Player1;
	private GameObject Player2;
	private Player _p1;
	private Player _p2;
	private Rigidbody _rb;
	private Vector3 _moveVector;

	private void Awake()
	{
		Player1 = GameObject.FindGameObjectWithTag("Player1");
		Player2 = GameObject.FindGameObjectWithTag("Player2");
		_p1 = ReInput.players.GetPlayer(0);
		_p2 = ReInput.players.GetPlayer(1);
		_rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!_canControl) return;
		_processP1Control();
		_processP2Control();
	}

	private void _processP2Control()
	{
		float moveHori = _p2.GetAxis("Move Horizontal");
		float moveVerti = _p2.GetAxis("Move Vertical");
		if (!Mathf.Approximately(moveHori, 0f) || !Mathf.Approximately(moveVerti, 0f))
		{
			Transform target = LookTarget.transform.GetChild(0);
			Vector3 relativePos = target.position - transform.position;

			LookTarget.transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(moveHori, moveVerti) * Mathf.Rad2Deg, transform.eulerAngles.z);

			Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
			Quaternion tr = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * MaxLookAtRotationDelta);
			transform.rotation = tr;
		}
	}

	private void _processP1Control()
	{
		float moveHori = _p1.GetAxis("Move Horizontal");
		float moveVerti = _p1.GetAxis("Move Vertical");
		_moveVector.x = moveHori * MaxMoveSpeed;
		_moveVector.y = _rb.velocity.y;
		_moveVector.z = moveVerti * MaxMoveSpeed;
		_rb.velocity = _moveVector;

	}

	private void _OnUltCalled()
	{
		if (!_called)
		{
			_called = true;
			_timeFirstCalled = Time.time;
		}
		else
		{
			if (Mathf.Abs(Time.time - _timeFirstCalled) < Tolerance)
			{
				// Successfully Called
				Vector3 CalledPosition = (Player1.transform.position + Player2.transform.position) / 2 + new Vector3(0f, 5f);
				transform.position = CalledPosition;
				_rb.isKinematic = false;
				_rb.velocity = new Vector3(0f, -20f);
				_canControl = true;
				_DisbalePlayer(Player1);
				_DisbalePlayer(Player2);
			}
			else
			{
				// Failed Called
				_called = false;
				_timeFirstCalled = 0f;
			}
		}
	}

	private void _DisbalePlayer(GameObject P)
	{
		P.transform.position = new Vector3(0f, 2000f);
		P.GetComponent<Rigidbody>().isKinematic = true;
		var Abilities = P.GetComponents<Ability>();
		foreach (var a in Abilities)
		{
			a.enabled = false;
		}
		P.GetComponent<PlayerController_SS>().enabled = false;
	}

	private void OnEnable()
	{
		EventManager.StartListening("CallUlt", _OnUltCalled);
	}

	private void OnDisable()
	{
		EventManager.StopListening("CallUlt", _OnUltCalled);
	}
}
