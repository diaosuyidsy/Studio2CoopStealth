using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
	private Camera _cam;
	private void Awake()
	{
		_cam = Camera.main;
	}
	// Update is called once per frame
	void Update()
	{
		transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
	}
}
