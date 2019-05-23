using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;
using Rewired;
using UnityEditor;
using DestroyIt;
using cakeslice;

public class Ability_Assasination : Ability
{
	public float Range = 1.5f;
	public float Angle = 60f;
	public bool isPlayingAnimation = false;
	public string playAnimation;
	protected vThirdPersonInput tpInput;
	[SerializeField] private LayerMask EnemyMask;
	private GameObject _interactableObject;
	private void Start()
	{
		tpInput = GetComponent<vThirdPersonInput>();
	}
	private void Update()
	{
		// First Get the nearby enemy, decide which one to interact
		_getNearbyEnemy();
		// And then see if stealth kill cd is complete, if so then can kill
		bool coolDownComplete = Time.time > nextReadyTime;
		if (coolDownComplete)
		{
			if (_player.GetButtonDown(ButtonName)) OnPressedDownAbility();
		}
		else CoolDown();
	}

	void ApplyPlayerSettings()
	{

		tpInput.cc._rigidbody.useGravity = false;               // disable gravity of the player
		tpInput.cc._rigidbody.velocity = Vector3.zero;
		tpInput.cc.isGrounded = true;                           // ground the character so that we can run the root motion without any issues
		tpInput.cc.animator.SetBool("IsGrounded", true);        // also ground the character on the animator so that he won't float after finishes the climb animation
		tpInput.cc.animator.SetInteger("ActionState", 1);       // set actionState 1 to avoid falling transitions     
		tpInput.cc._capsuleCollider.isTrigger = true;           // disable the collision of the player if necessary 

		tpInput.enabled = false;
		tpInput.cc.enabled = false;
	}

	void ResetPlayerSettings()
	{
		tpInput.enabled = true;
		tpInput.cc.enabled = true;
		tpInput.cc.EnableGravityAndCollision(0f);             // enable again the gravity and collision
		tpInput.cc.animator.SetInteger("ActionState", 0);     // set actionState 1 to avoid falling transitions

	}
	void OnAnimatorMove()
	{
		if (!isPlayingAnimation) return;
		if (!tpInput.cc.customAction)
		{
			// enable movement using root motion
			transform.rotation = tpInput.cc.animator.rootRotation;
		}
		transform.position = tpInput.cc.animator.rootPosition;
	}

	public override void OnPressedDownAbility()
	{
		if (_isUsingOtherAbility) return;
		if (_interactableObject == null) return;
		if (!SpendEnergy()) return;
		nextReadyTime = BaseCoolDown + Time.time;
		coolDownTimeLeft = BaseCoolDown;
		EventManager.TriggerEvent($"Player{PlayerID}InAbility");

		tpInput.cc.animator.CrossFadeInFixedTime(playAnimation, 0.1f);
		ApplyPlayerSettings();
		isPlayingAnimation = true;
		StartCoroutine(waitKill());

	}

	IEnumerator waitKill()
	{
		yield return new WaitForSeconds(1.0f);
		if (_interactableObject != null)
		{
			//GameObject substitude = Instantiate(_interactableObject, _interactableObject.transform.position, _interactableObject.transform.rotation);

			Destructible destObj = _interactableObject.GetComponentInChildren<Destructible>();
			if (destObj != null)
			{
				ImpactDamage meleeImpact = new ImpactDamage()
				{
					DamageAmount = 50,
					AdditionalForce = 0f,
					AdditionalForcePosition = transform.position,
					AdditionalForceRadius = 0f
				};
				destObj.ApplyDamage(meleeImpact);
			}
			//substitude.SetActive(false);
			//substitude = null;
			if (_interactableObject.GetComponentInChildren<Outline>() != null)
				_interactableObject.GetComponentInChildren<Outline>().DisableOutline();
			_interactableObject.SetActive(false);
			_interactableObject = null;
		}
		isPlayingAnimation = false;
		EventManager.TriggerEvent($"Player{PlayerID}Free");
		ResetPlayerSettings();
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
		if (_interactableObject != null && smallestGO != _interactableObject && _interactableObject.GetComponentInChildren<Outline>() != null) _interactableObject.GetComponentInChildren<Outline>().DisableOutline();
		_interactableObject = smallestGO;
		if (_interactableObject != null && _interactableObject.GetComponentInChildren<Outline>() != null) _interactableObject.GetComponentInChildren<Outline>().EnableOutline();

	}

	private float _angleBetween(GameObject go)
	{
		Vector3 targetDir = go.transform.position - transform.position;
		float angle = Vector3.Angle(targetDir, transform.forward);
		return angle;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(Ability_Assasination))]
public class DrawSolidArc : Editor
{
	private void OnSceneGUI()
	{
		Handles.color = Color.yellow;
		Ability_Assasination AA = (Ability_Assasination)target;
		Handles.DrawSolidArc(AA.transform.position - new Vector3(0f, 0.5f, 0f), AA.transform.up, AA.transform.forward, AA.Angle, AA.Range);
		Handles.DrawSolidArc(AA.transform.position - new Vector3(0f, 0.5f, 0f), AA.transform.up, AA.transform.forward, -AA.Angle, AA.Range);
	}
}
#endif
