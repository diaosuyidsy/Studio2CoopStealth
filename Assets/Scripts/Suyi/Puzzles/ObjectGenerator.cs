using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
	public GameObject SpawnedObjectPrefab;
	/// <summary>
	/// Checking would destroy other instances initialized
	/// </summary>
	public bool OnlyAllowOnceInstanceInWorld = true;
	private GameObject SpawnedObject;

	private void Awake()
	{
		Debug.Assert(SpawnedObjectPrefab != null);
	}

	/// <summary>
	/// SpawnObject spawns the object at the spawner
	/// </summary>
	public void SpawnObject()
	{
		SpawnObjectWorldPosition(transform.position);
	}

	/// <summary>
	/// SpawnObjectWorldPosition spawns the object at world position specified
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public void SpawnObjectWorldPosition(Vector3 position)
	{
		if (OnlyAllowOnceInstanceInWorld && SpawnedObject != null)
		{
			SpawnedObject.transform.position = Vector3.zero;
			Destroy(SpawnedObject, 0.1f);
		}
		SpawnedObject = Instantiate(SpawnedObjectPrefab, position, Quaternion.identity);
	}

	/// <summary>
	/// This function Spawns Object relative to the spawner
	/// </summary>
	/// <param name="offsetX"></param>
	/// <param name="offsetY"></param>
	/// <param name="offsetZ"></param>
	public void SpawnObject(Vector3 offset)
	{
		var pos = transform.position;
		pos += offset;
		SpawnObjectWorldPosition(pos);
	}
}
