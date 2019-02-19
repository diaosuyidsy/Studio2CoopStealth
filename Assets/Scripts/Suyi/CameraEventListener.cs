using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEventListener : MonoBehaviour
{
	private Camera _camera;

	private void Awake()
	{
		_camera = GetComponent<Camera>();
	}

	private void OnEnable()
	{
		EventManager.StartListening("TimeSwitch", _onSwitchTime);
	}

	private void OnDisable()
	{
		EventManager.StopListening("TimeSwitch", _onSwitchTime);
	}

	private void _onSwitchTime()
	{
		_camera.depth = _camera.depth == -1 ? -2 : -1;
	}
}
