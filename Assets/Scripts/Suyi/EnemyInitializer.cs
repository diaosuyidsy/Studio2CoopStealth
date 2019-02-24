using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

[RequireComponent(typeof(BehaviorTree))]
public class EnemyInitializer : MonoBehaviour
{
	#region Publicly Set Variables
	public GameObject DiscoverBar;
	public GameObject AwarenessBar;
	public Transform PatrolPointsHolder;
	public Transform StandStillPoint;
	public Vector3 StandStillRotation;
	public GameObject ExclamationMark;
	public string PlayerTag = "Player";
	#endregion

	private List<GameObject> _pps;
	private GameObject _player;
	private BehaviorTree _bt;

	private void Awake()
	{
		_bt = GetComponent<BehaviorTree>();
		_pps = new List<GameObject>();
		_player = GameObject.FindGameObjectWithTag(PlayerTag);
		_bt.SetVariableValue("Player", _player);
		_bt.SetVariableValue("DiscoverBar", DiscoverBar);
		_bt.SetVariableValue("StandStillPosition", new Vector3(StandStillPoint.position.x, StandStillPoint.position.y, StandStillPoint.position.z));
		_bt.SetVariableValue("StandStillRotation", StandStillRotation);
		for (int i = 0; i < PatrolPointsHolder.childCount; i++)
		{
			GameObject go = new GameObject();
			go.transform.position = PatrolPointsHolder.GetChild(i).position;
			_pps.Add(go);
		}
		_bt.SetVariableValue("Patrol Points", _pps);
		_bt.SetVariableValue("Excalmation Mark", ExclamationMark);
		_bt.SetVariableValue("Awareness Bar", AwarenessBar);
	}

}
