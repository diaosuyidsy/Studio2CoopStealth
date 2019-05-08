using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestruct : MonoBehaviour
{
	private ParticleSystem ps;


	public void Start()
	{
		ps = GetComponent<ParticleSystem>();
		Destroy(gameObject, ps.startLifetime);
	}
}
