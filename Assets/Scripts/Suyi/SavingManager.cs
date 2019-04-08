using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

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
		SavingPoints = GetComponentsInChildren<Transform>();
		Player1 = GameObject.FindGameObjectWithTag("Player1");
		Player2 = GameObject.FindGameObjectWithTag("Player2");
		SavingIndex = 0;
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
