using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System.Linq;

public class SavingManager : MonoBehaviour
{
	public static SavingManager SM;
	public int SavingIndex { get; set; }
	private Transform[] SavingPoints;
	private GameObject Player1;
	private GameObject Player2;

	private void Awake()
	{
		SM = this;
		var temp = GetComponentsInChildren<SavingPoint>();
		SavingPoints = new Transform[temp.Length];
		for (int i = 0; i < temp.Length; i++)
		{
			SavingPoints[i] = temp[i].transform;
		}
		Player1 = GameObject.FindGameObjectWithTag("Player1");
		Player2 = GameObject.FindGameObjectWithTag("Player2");
		SavingIndex = 0;
		Tinylytics.AnalyticsManager.LogCustomMetric("Open Level", "");
	}

	IEnumerator Revive(float time)
	{
		Player1.SetActive(false);
		Player2.SetActive(false);

		yield return new WaitForSeconds(time);

		EventManager.TriggerEvent("OnRewind");

		Player1.transform.position = SavingPoints[SavingIndex].GetComponent<SavingPoint>().Player1SpawnPoint;
		Player2.transform.position = SavingPoints[SavingIndex].GetComponent<SavingPoint>().Player2SpawnPoint;

		Player1.SetActive(true);
		Player2.SetActive(true);
	}

	private void OnEnable()
	{
		EventManager.StartListening("PlayerDied", () =>
		{
			ReInput.players.GetPlayer(0).SetVibration(0, 1.0f, 0.25f);
			ReInput.players.GetPlayer(1).SetVibration(0, 1.0f, 0.25f);
			StartCoroutine(Revive(2f));
		});
	}

	private void OnDisable()
	{
		EventManager.StopListening("PlayerDied", () =>
		{
			ReInput.players.GetPlayer(0).SetVibration(0, 1.0f, 0.25f);
			ReInput.players.GetPlayer(1).SetVibration(0, 1.0f, 0.25f);
			StartCoroutine(Revive(2f));
		});
	}
}
