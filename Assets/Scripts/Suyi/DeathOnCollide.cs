﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathOnCollide : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			other.gameObject.SetActive(false);
		}
	}
}
