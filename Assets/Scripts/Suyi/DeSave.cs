using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeSave : MonoBehaviour
{
	private SavingPoint _sp;

	private void Awake()
	{
		_sp = transform.parent.GetComponent<SavingPoint>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player1")) _sp.P1Entered = false;
		if (other.CompareTag("Player2")) _sp.P2Entered = false;
	}
}
