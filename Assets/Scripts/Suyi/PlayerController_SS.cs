using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController_SS : MonoBehaviour
{
	[Header("Player Controller is supposed to be either 0 or 1")]
	public int PlayerID;
	public float MaxMoveSpeed = 10f;
	public float MaxLookAtRotationDelta = 4f;
	public float JumpForce = 10f;
	public LayerMask JumpMask;
	public LayerMask EnemyMask;
	public Transform LookTarget;

	#region Player Controller Section
	private Player _player;
	private Rigidbody _rb;
	private Vector3 _moveVector;
	private float _horizontal;
	private float _vertical;
	private bool _jump;
	private bool _specialAction;
	private bool _stealthKill;
	private float _distToGround;
	private GameObject _interactableObject;
	#endregion

	private void Awake()
	{
		_player = ReInput.players.GetPlayer(PlayerID);
		_rb = GetComponent<Rigidbody>();
		_distToGround = GetComponent<CapsuleCollider>().bounds.extents.y;
	}

	// Update is called once per frame
	void Update()
	{
		_getInput();
		_processMovement();
		_checkInteraction();
		_processAction();
	}

	private void _getInput()
	{
		_horizontal = _player.GetAxis("Move Horizontal");
		_vertical = _player.GetAxis("Move Vertical");
		_jump = _player.GetButtonDown("Jump");
		_specialAction = _player.GetButtonDown("Special Usage");
		_stealthKill = _player.GetButtonDown("Stealth Kill");
	}

	private void _processMovement()
	{
		_moveVector.x = _horizontal * MaxMoveSpeed;
		_moveVector.y = _rb.velocity.y;
		_moveVector.z = _vertical * MaxMoveSpeed;
		_rb.velocity = _moveVector;
		if (!Mathf.Approximately(_horizontal, 0f) || !Mathf.Approximately(_vertical, 0f))
		{
			Transform target = LookTarget.transform.GetChild(0);
			Vector3 relativePos = target.position - transform.position;

			LookTarget.transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(_horizontal, _vertical) * Mathf.Rad2Deg, transform.eulerAngles.z);

			Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
			Quaternion tr = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * MaxLookAtRotationDelta);
			transform.rotation = tr;
		}
	}

	private void _processAction()
	{
		if (_jump && IsGrounded()) _rb.AddForce(new Vector3(0f, JumpForce, 0f), ForceMode.Impulse);
		if (_specialAction) EventManager.TriggerEvent("TimeSwitch");
		if (_stealthKill)
		{
			if (_interactableObject != null) _interactableObject.SetActive(false);
		}
	}

	private void _checkInteraction()
	{
		// First check if there is a enemy to kill
		Collider[] hitcolliders = Physics.OverlapSphere(transform.position, 1.5f, EnemyMask);
		if (hitcolliders.Length == 0) return;
		float smallestAngle = Mathf.Infinity;
		GameObject smallestGO = null;
		for (int i = 0; i < hitcolliders.Length; i++)
		{
			var hit = hitcolliders[i].gameObject;
			float angle = _angleBetween(hit);
			if (angle < 60f && angle < smallestAngle)
			{
				smallestGO = hit;
			}
		}
		if (smallestGO == null && _interactableObject != null)
		{
			_interactableObject.GetComponent<EnemyInitializer>().SetKillable(false);
			_interactableObject = null;
		}
		if (smallestGO != null)
		{
			_interactableObject = smallestGO;
			_interactableObject.GetComponent<EnemyInitializer>().SetKillable(true);
		}

	}

	public void SetInteractableObject(GameObject go)
	{
		_interactableObject = go;
	}

	public bool IsGrounded()
	{
		RaycastHit hit;
		bool result = Physics.SphereCast(transform.position, 0.3f, Vector3.down, out hit, _distToGround, JumpMask);

		return result;
	}

	private float _angleBetween(GameObject go)
	{
		Vector3 targetDir = go.transform.position - transform.position;
		float angle = Vector3.Angle(targetDir, transform.forward);
		return angle;
	}
}
