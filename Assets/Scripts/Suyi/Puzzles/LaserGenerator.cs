using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGenerator : MonoBehaviour
{
	public int Length = 10;
	public int Width = 5;
	public float horizontalstep = 0.5f;
	public float verticalstep = 0.5f;
	public GameObject LaserPrefab;

	private GameObject[] Laser;

	private void Awake()
	{
		Laser = new GameObject[Length * Width];
		for (int i = 0; i < Length; i++)
		{
			for (int j = 0; j < Width; j++)
			{
				Laser[i + j] = Instantiate(LaserPrefab, transform);
				Laser[i + j].transform.localPosition = new Vector3(i * horizontalstep, j * verticalstep, 0f);
			}
		}
	}

	private void Update()
	{

	}
}
