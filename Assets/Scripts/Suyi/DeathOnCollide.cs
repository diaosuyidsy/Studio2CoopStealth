using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathOnCollide : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		print(other.name);
		if (other.CompareTag("Enemy") || other.CompareTag("Player1") || other.CompareTag("Player2"))
		{
			other.gameObject.SetActive(false);
		}
		if (other.CompareTag("Player1") || other.CompareTag("Player2"))
		{
			EventManager.TriggerEvent("PlayerDied");
		}
		if (other.CompareTag("Player"))
		{
			var rewindable = other.GetComponentInParent<Rewindable>();
			if (rewindable != null)
				rewindable.OnRewind();
		}
	}
}
