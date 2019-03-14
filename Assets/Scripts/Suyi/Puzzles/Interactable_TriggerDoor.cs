using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_TriggerDoor : Interactable
{
	public Vector3 FinalPositionOffset = Vector3.one;
	public float DoorOpenMovingSpeed = 2f;
	public float DoorCloseMovingSpeed = 4f;

	private Vector3 _DoorFinalPosition;
	private Vector3 _DoorInitialPosition;
	private int _triggerAmountTotal;
	private int _triggerAmountCur;
	private bool _openingdoor
	{
		get
		{
			return _triggerAmountCur >= _triggerAmountTotal;
		}
	}

	private void Awake()
	{
		_DoorInitialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		_DoorFinalPosition = new Vector3(transform.position.x + FinalPositionOffset.x,
			transform.position.y + FinalPositionOffset.y,
			transform.position.z + FinalPositionOffset.z);
		_triggerAmountTotal = transform.parent.childCount - 1;
	}

	private void Update()
	{
		if (_openingdoor)
		{
			transform.position = Vector3.MoveTowards(transform.transform.position, _DoorFinalPosition, Time.deltaTime * DoorOpenMovingSpeed);
		}
		else
		{
			transform.position = Vector3.MoveTowards(transform.transform.position, _DoorInitialPosition, Time.deltaTime * DoorCloseMovingSpeed);
		}
	}

	public override void OnInteractDown(Object param = null)
	{
		base.OnInteractDown(param);
		_triggerAmountCur++;
	}

	public override void OnInteractUp(Object param = null)
	{
		base.OnInteractUp(param);
		_triggerAmountCur--;
	}
}
