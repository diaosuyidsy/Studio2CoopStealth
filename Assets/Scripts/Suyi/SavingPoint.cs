using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingPoint : MonoBehaviour
{
	public Vector3 P1SpawnOffset;
	public Vector3 P2SpawnOffset;

	public Vector3 Player1SpawnPoint
	{
		get
		{
			return transform.position + P1SpawnOffset;
		}
	}

	public Vector3 Player2SpawnPoint
	{
		get
		{
			return transform.position + P2SpawnOffset;
		}
	}

	private bool P1Entered;
	private bool P2Entered;
	private bool recorded;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player1")) P1Entered = true;
		if (other.CompareTag("Player2")) P2Entered = true;
		if (!recorded && P1Entered && P2Entered)
		{
			recorded = true;
			SavingManager.SM.SavingIndex++;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(transform.position + P1SpawnOffset, 1);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position + P2SpawnOffset, 1);
	}
}
