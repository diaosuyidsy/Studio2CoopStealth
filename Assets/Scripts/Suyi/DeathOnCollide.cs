using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathOnCollide : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy") || other.CompareTag("Player1") || other.CompareTag("Player2"))
		{
			other.gameObject.SetActive(false);
		}
		if (other.CompareTag("Player1") || other.CompareTag("Player2"))
		{
			EventManager.TriggerEvent("PlayerDied");
		}
	}
}
