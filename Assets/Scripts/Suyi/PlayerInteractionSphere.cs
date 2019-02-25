using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionSphere : MonoBehaviour
{
	private List<GameObject> _interactable;
	private PlayerController_SS _pcs;

	private void Awake()
	{
		_pcs = GetComponentInParent<PlayerController_SS>();
		StartCoroutine(UD());
	}

	IEnumerator UD()
	{
		while (true)
		{
			yield return new WaitForFixedUpdate();

		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			_interactable.Add(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (_interactable.Contains(other.gameObject))
		{
			_interactable.Remove(other.gameObject);
		}
	}
}
