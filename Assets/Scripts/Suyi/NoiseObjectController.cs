using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
public class NoiseObjectController : MonoBehaviour
{
	public float Range = 5f;
	public GameObject SoundRippleEffectPrefab;
	private AudioSource _as;

	private void Awake()
	{
		_as = GetComponent<AudioSource>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			EmitSound(Range);
		}
	}

	public void EmitSound(float _range)
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, _range);
		int i = 0;
		while (i < hitColliders.Length)
		{
			var bt = hitColliders[i].gameObject.GetComponent<BehaviorTree>();
			if (bt != null)
			{
				bt.SendEvent<object>("StrangeSound", gameObject);
			}
			i++;
		}
		if (_as != null)
		{
			_as.Play();
		}
		if (SoundRippleEffectPrefab != null)
		{
			GameObject particle = Instantiate(SoundRippleEffectPrefab, transform.position, SoundRippleEffectPrefab.transform.rotation);
			particle.GetComponent<ParticleSystem>().startSize = Range * 3f;
		}
	}
}
