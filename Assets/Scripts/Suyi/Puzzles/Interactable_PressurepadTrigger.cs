using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_PressurepadTrigger : Interactable_Trigger
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag("Player1") || collision.collider.CompareTag("Player2"))
		{
			OnInteractDown(collision.collider.gameObject);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.collider.CompareTag("Player1") || collision.collider.CompareTag("Player2"))
		{
			OnInteractUp(collision.collider.gameObject);
		}
	}
}
