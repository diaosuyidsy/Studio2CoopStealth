using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinylyticsDataCollection : MonoBehaviour
{

	private void _recordData()
	{
		Tinylytics.AnalyticsManager.LogCustomMetric("Player Died", "");
	}

	private void OnEnable()
	{
		EventManager.StartListening("PlayerDied", _recordData);
	}

	private void OnDisable()
	{
		EventManager.StopListening("PlayerDied", _recordData);
	}
}
