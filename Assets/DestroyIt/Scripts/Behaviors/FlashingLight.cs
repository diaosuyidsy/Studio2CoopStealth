using UnityEngine;
using System.Collections;

public class FlashingLight : MonoBehaviour
{
	public float flashInterval = 0.5f;
	
    private Light flashingLight;

	void Start()
    {
		flashingLight = GetComponent<Light>();
		StartCoroutine(Flashing());
	}
	
	IEnumerator Flashing()
	{
		while (true)
		{
			yield return new WaitForSeconds(flashInterval);
			flashingLight.enabled = !flashingLight.enabled;
		}
	}
}