using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_HoldOpenDoor : Interactable
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
		if (state == InteractableState.Idle)
		{
			Door.position = Vector3.MoveTowards(Door.transform.position, _DoorInitialPosition, Time.deltaTime * DoorCloseMovingSpeed);
		}
	}

	public override void OnInteracting(Object param = null)
	{
		Door.position = Vector3.MoveTowards(Door.transform.position, _DoorFinalPosition, Time.deltaTime * DoorOpenMovingSpeed);
	}

	public override void OnInteractUp(Object param = null)
	{
		base.OnInteractUp(param);

	}
}
