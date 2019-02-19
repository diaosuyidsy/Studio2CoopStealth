using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLenseListener : MonoBehaviour
{
	public GameObject CameraLense;

	private void OnEnable()
	{
		EventManager.StartListening("TimeSwitch", _onTimeSwitch);
	}

	private void OnDisable()
	{
		EventManager.StopListening("TimeSwitch", _onTimeSwitch);
	}

	private void _onTimeSwitch()
	{
		// Switch active status
		CameraLense.SetActive(!CameraLense.activeSelf);
	}
}
