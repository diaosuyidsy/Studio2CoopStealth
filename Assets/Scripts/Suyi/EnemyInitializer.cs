using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

[RequireComponent(typeof(BehaviorTree))]
public class EnemyInitializer : MonoBehaviour
{
	#region Publicly Set Variables
	public Transform PatrolPointsHolder;
	public Transform StandStillPoint;
	public GameObject ExclamationMark;
	public string PlayerTag = "Player";
	public GameObject KillableImage;
	#endregion

	private List<GameObject> _pps;
	private GameObject _player;
	private BehaviorTree _bt;

	private void Awake()
	{
		_bt = GetComponent<BehaviorTree>();
		_pps = new List<GameObject>();
		_bt.SetVariableValue("StandStillPosition", new Vector3(StandStillPoint.position.x, StandStillPoint.position.y, StandStillPoint.position.z));
		_bt.SetVariableValue("StandStillRotation", transform.eulerAngles);
		for (int i = 0; i < PatrolPointsHolder.childCount; i++)
		{
			GameObject go = new GameObject();
			go.transform.position = PatrolPointsHolder.GetChild(i).position;
			_pps.Add(go);
		}
		_bt.SetVariableValue("Patrol Points", _pps);
		_bt.SetVariableValue("Excalmation Mark", ExclamationMark);
	}

	public void SetKillable(bool t)
	{
		KillableImage.SetActive(t);
	}

	private void _unlockplayer()
	{
		_bt.SetVariableValue("PlayerLocked", false);
	}

	private void OnEnable()
	{
		EventManager.StartListening("PlayerDied", _unlockplayer);
	}

	private void OnDisable()
	{
		EventManager.StopListening("PlayerDied", _unlockplayer);
	}

}
