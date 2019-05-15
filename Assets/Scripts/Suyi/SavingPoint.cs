using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class SavingPoint : MonoBehaviour
{
	public Vector3 P1SpawnOffset;
	public Vector3 P2SpawnOffset;
	public float CameraTransitionTime = 1f;
	[Tooltip("Used to set Camera Wiggle Room in X Direction")]
	public float XOffset = 2f;
	[Tooltip("Used to set Camera Wiggle Room in Z Direction")]
	public float ZOffset = 2f;
	public UnityEvent OnEnterSavingPoint;

	private Transform _camPosition;

	public Vector3 Player1SpawnPoint
	{
		get
		{
			return transform.position + P1SpawnOffset;
		}
	}

	public Vector3 Player2SpawnPoint
	{
		get
		{
			return transform.position + P2SpawnOffset;
		}
	}

	public bool P1Entered;
	public bool P2Entered;
	private bool recorded;

	private int thisSavingIndex;
	private CameraController _mainCamControl;

	private void Awake()
	{
		thisSavingIndex = transform.GetSiblingIndex();
		_mainCamControl = Camera.main.GetComponent<CameraController>();
		_camPosition = transform.GetChild(0);
		Debug.Assert(_camPosition != null);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player1")) P1Entered = true;
		if (other.CompareTag("Player2")) P2Entered = true;
		if (!recorded && P1Entered && P2Entered)
		{
			recorded = true;
			SavingManager.SM.SavingIndex = thisSavingIndex;
			//Camera.main.transform.GetComponent<CameraController>().AdjustHeight = CameraHeight;
			OnEnterSavingPoint.Invoke();
			Tinylytics.AnalyticsManager.LogCustomMetric("Current Level", (thisSavingIndex + 1).ToString());
			_mainCamControl.SetCameraPosRot(_camPosition, transform.GetChild(1), CameraTransitionTime, XOffset, ZOffset);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(transform.position + P1SpawnOffset, 1);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position + P2SpawnOffset, 1);
	}
}
#if UNITY_EDITOR
[CustomEditor(typeof(SavingPoint))]
public class DrawWireRectangle : Editor
{
	private void OnSceneGUI()
	{
		Handles.color = Color.yellow;
		SavingPoint AA = (SavingPoint)target;
		Handles.DrawWireCube(AA.transform.GetChild(0).position, new Vector3(AA.XOffset * 2f, 0.5f, AA.ZOffset * 2f));
	}
}
#endif
