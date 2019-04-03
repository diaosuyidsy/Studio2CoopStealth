using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
	private LineRenderer lr;
	// Use this for initialization
	void Start()
	{
		lr = GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		lr.SetPosition(0, transform.position);
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit))
		{
			if (hit.collider)
			{
				lr.SetPosition(1, hit.point);
			}
			if (hit.collider.CompareTag("Player1") || hit.collider.CompareTag("Player2") || hit.collider.CompareTag("Enemy"))
			{
				hit.collider.gameObject.SetActive(false);
			}
			if (hit.collider.CompareTag("Player1") || hit.collider.CompareTag("Player2"))
			{
				EventManager.TriggerEvent("PlayerDied");
			}
		}
		else lr.SetPosition(1, transform.forward * 5000);
	}
}