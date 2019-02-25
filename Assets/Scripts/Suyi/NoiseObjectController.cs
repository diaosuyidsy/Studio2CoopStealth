using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
public class NoiseObjectController : MonoBehaviour
{
	public float Range = 5f;
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, Range);
			int i = 0;
			while (i < hitColliders.Length)
			{
				var bt = hitColliders[i].gameObject.GetComponent<BehaviorTree>();
				var ei = hitColliders[i].gameObject.GetComponent<EnemyInitializer>();
				if (bt != null)
				{
					bt.SendEvent<object>("StrangeSound", gameObject);
				}
				i++;
			}
		}
	}
}
