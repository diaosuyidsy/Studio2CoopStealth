using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
	public bool IsOccupied { get; protected set; }

	protected InteractableState state;
	/// <summary>
	/// On Interact Down, similar to on button down
	/// </summary>
	/// <param name="param"></param>
	public virtual void OnInteractDown(Object param = null)
	{
		IsOccupied = true;
		state = InteractableState.Interacting;
	}

	/// <summary>
	/// On Interacting, similar to on button
	/// </summary>
	/// <param name="param"></param>
	public virtual void OnInteracting(Object param = null) { }

	/// <summary>
	/// On Interact Up, similar to on button up
	/// </summary>
	/// <param name="param"></param>
	public virtual void OnInteractUp(Object param = null)
	{
		IsOccupied = false;
		state = InteractableState.Idle;
	}
}

public enum InteractableState
{
	Idle,
	Interacting,
}
