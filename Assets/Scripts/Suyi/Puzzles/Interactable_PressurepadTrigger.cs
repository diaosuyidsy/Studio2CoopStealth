using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_PressurepadTrigger : Interactable_Trigger
{

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player1") || other.CompareTag("Player2") || other.CompareTag("TriggerInteractable"))
		{
			OnInteractDown(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player1") || other.CompareTag("Player2") || other.CompareTag("TriggerInteractable"))
		{
			OnInteractUp(other.gameObject);
		}
	}

}
