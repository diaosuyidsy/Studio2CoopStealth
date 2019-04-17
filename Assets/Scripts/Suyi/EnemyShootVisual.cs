using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class EnemyShootVisual : MonoBehaviour
{
	public float timeBetweenBullets = 0.3f;        // The time between each shot.
	public float range = Mathf.Infinity;            // The distance the gun can fire.
	public float BulletSpeed = 500f;
	public GameObject BulletPrefab;

	float timer;                                    // A timer to determine when to fire.
	Ray shootRay;                                   // A ray from the gun end forwards.
	RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
	public LayerMask shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
	Light gunLight;                                 // Reference to the light component.
	float effectsDisplayTime = 1f;                // The proportion of the timeBetweenBullets that the effects will display for.
	private GameObject _bulletInstance;
	private Vector3 _bulletTargetLocation;

	void Awake()
	{
		// Set up the references.
		gunLight = GetComponent<Light>();
		_bulletTargetLocation = Vector3.zero;
	}

	void Update()
	{
		// Add the time since Update was last called to the timer.
		timer += Time.deltaTime;

		// If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
		if (timer >= timeBetweenBullets)
		{
			// ... disable the effects.
			DisableEffects();
		}

		if (_bulletInstance != null && _bulletTargetLocation != Vector3.zero)
		{
			_bulletInstance.transform.position = Vector3.MoveTowards(_bulletInstance.transform.position, _bulletTargetLocation, Time.deltaTime * BulletSpeed);
			if (Vector3.Magnitude(_bulletInstance.transform.position - _bulletTargetLocation) < 0.1f)
			{
				Destroy(_bulletInstance);
				_bulletInstance = null;
				_bulletTargetLocation = Vector3.zero;
			}
		}
	}

	public void DisableEffects()
	{
		// Disable the line renderer and the light.
		gunLight.enabled = false;
	}

	public void Shoot()
	{
		// Reset the timer.
		timer = 0f;

		// Play the gun shot audioclip.
		//gunAudio.Play();

		// Enable the light.
		gunLight.enabled = true;


		// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
		shootRay.origin = transform.position;
		shootRay.direction = transform.forward;

		_bulletInstance = Instantiate(BulletPrefab, transform.position, Quaternion.identity);

		// Perform the raycast against gameobjects on the shootable layer and if it hits something...
		if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
		{
			// Set the second position of the line renderer to the point the raycast hit.
			_bulletTargetLocation = shootHit.point;
		}
		// If the raycast didn't hit anything on the shootable layer...
		else
		{
			// ... set the second position of the line renderer to the fullest extent of the gun's range.
			_bulletTargetLocation = shootRay.origin + shootRay.direction * range;
		}
	}
}
