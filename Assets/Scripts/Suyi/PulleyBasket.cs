using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PulleyBasket : MonoBehaviour
{
	public PulleyBasket OtherBasket;
	public GameObject Pulley;
	public float _massOnObject;

	private LineRenderer _lr;
	private float _disToPulley;
	private float _upMostYPos;
	private float _downMostYPos;

	private void Awake()
	{
		_lr = GetComponent<LineRenderer>();
		_disToPulley = Pulley.transform.position.y - transform.position.y;
		_upMostYPos = transform.position.y + _disToPulley;
		_downMostYPos = transform.position.y - _disToPulley;
	}

	private void Update()
	{
		_renderLine();
		if (OtherBasket._massOnObject < _massOnObject && transform.position.y > _downMostYPos)
		{
			transform.position -= new Vector3(0f, Time.deltaTime * 5f, 0f);
		}
		else if (OtherBasket._massOnObject > _massOnObject && transform.position.y < _upMostYPos)
		{
			transform.position += new Vector3(0f, Time.deltaTime * 5f, 0f);
		}
	}

	private void _renderLine()
	{
		_lr.SetPosition(0, transform.position);
		_lr.SetPosition(1, Pulley.transform.position);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag("Player1") || collision.collider.CompareTag("Player2") || collision.collider.CompareTag("Item"))
		{
			collision.collider.transform.parent = transform;
			var rb = collision.collider.GetComponent<Rigidbody>();
			_massOnObject += rb.mass;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.collider.CompareTag("Player1") || collision.collider.CompareTag("Player2"))
		{
			collision.collider.transform.parent = null;
			var rb = collision.collider.GetComponent<Rigidbody>();
			_massOnObject -= rb.mass;
		}

		if (collision.collider.CompareTag("Item"))
		{
			var rb = collision.collider.GetComponent<Rigidbody>();
			_massOnObject -= rb.mass;
		}
	}
}
