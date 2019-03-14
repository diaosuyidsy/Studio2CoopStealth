using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_StandOnDoorSingle : Interactable
{
	public Transform Door;
	public Vector3 FinalPositionOffset = Vector3.one;
	public float DoorOpenMovingSpeed = 2f;
	public float DoorCloseMovingSpeed = 4f;

	private Vector3 _DoorFinalPosition;
	private Vector3 _DoorInitialPosition;

	private void Awake()
	{
		_DoorInitialPosition = new Vector3(Door.position.x, Door.position.y, Door.position.z);
		_DoorFinalPosition = new Vector3(Door.position.x + FinalPositionOffset.x,
			Door.position.y + FinalPositionOffset.y,
			Door.position.z + FinalPositionOffset.z);
	}

	private void Update()
	{
		if (state == InteractableState.Interacting)
		{
			OnInteracting();
		}
		else
		{
			Door.position = Vector3.MoveTowards(Door.transform.position, _DoorInitialPosition, Time.deltaTime * DoorCloseMovingSpeed);
		}
	}

	public override void OnInteracting(Object param = null)
	{
		Door.position = Vector3.MoveTowards(Door.transform.position, _DoorFinalPosition, Time.deltaTime * DoorOpenMovingSpeed);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag("Player1") || collision.collider.CompareTag("Player2"))
		{
			base.OnInteractDown();
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.collider.CompareTag("Player1") || collision.collider.CompareTag("Player2"))
		{
			base.OnInteractUp();
		}
	}
}
