using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_PressurepadTrigger : Interactable_Trigger
{

	private int num = 0;
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player1") || other.CompareTag("Player2") || other.CompareTag("TriggerInteractable"))
		{
			num++;
			if (num == 1)
				OnInteractDown(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player1") || other.CompareTag("Player2") || other.CompareTag("TriggerInteractable"))
		{
			num--;
			if (num == 0)
				OnInteractUp(other.gameObject);
		}
	}

	private void OnEnable()
	{
		EventManager.StartListening("OnRewind", () =>
		{
			num = 0;
			OnInteractUp(null);
		});
	}

	private void OnDisable()
	{
		EventManager.StopListening("OnRewind", () =>
		{
			num = 0;
			OnInteractUp(null);
		});
	}

}
