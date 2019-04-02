using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Rewindable : MonoBehaviour
{
	public int NextSavePointIndex;
	public UnityEvent OnRewindEvents;

	protected Vector3 OriginalPosition;
	protected Vector3 OriginalRotation;
	protected Vector3 OriginalScale;

	protected virtual void Awake()
	{
		OriginalPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		OriginalRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
		OriginalScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

	public virtual void OnRewind()
	{
		print("Rewind1");
		if (SavingManager.SM.SavingIndex > NextSavePointIndex) return;
		print("Rewind2");
		// First rewind the transform
		transform.position = OriginalPosition;
		transform.eulerAngles = OriginalRotation;
		transform.localScale = OriginalScale;

		// Next rewind the DoTween if we have
		if (OnRewindEvents != null) OnRewindEvents.Invoke();
	}

	protected virtual void OnEnable()
	{
		EventManager.StartListening("OnRewind", OnRewind);
	}

	protected virtual void OnDisable()
	{
		EventManager.StopListening("OnRewind", OnRewind);
	}
}
