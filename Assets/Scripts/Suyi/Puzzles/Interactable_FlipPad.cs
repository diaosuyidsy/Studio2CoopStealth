using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable_FlipPad : Interactable
{
	public UnityEvent OnPlayerEnter;
	public UnityEvent OnPlayerExit;
	public UnityEvent OnBothPlayerEnter;

	public int enteredplayernum = 0;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player1") || other.CompareTag("Player2"))
		{
			OnPlayerEnter.Invoke();
			enteredplayernum++;
			if (enteredplayernum >= 2) OnBothPlayerEnter.Invoke();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player1") || other.CompareTag("Player2"))
		{
			OnPlayerExit.Invoke();
			enteredplayernum--;
		}

	}
}
