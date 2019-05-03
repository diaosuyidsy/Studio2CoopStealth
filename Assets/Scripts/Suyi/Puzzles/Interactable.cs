using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
	public bool IsOccupied { get; protected set; }
	public UnityEvent OnInteractDownAction;
	public UnityEvent OnInteractingAction;
	public UnityEvent OnInteractUpAction;
	/// <summary>
	/// 1 can only be interacted by big player, 2 can be interacted by small
	/// 3 can be interacted by both
	/// </summary>
	public byte CanInteractPlayerNumber = 3;

	protected InteractableState state;
	/// <summary>
	/// On Interact Down, similar to on button down
	/// </summary>
	/// <param name="param"></param>
	public virtual void OnInteractDown(Object param = null)
	{
		IsOccupied = true;
		state = InteractableState.Interacting;
		OnInteractDownAction.Invoke();
	}

	/// <summary>
	/// On Interacting, similar to on button
	/// </summary>
	/// <param name="param"></param>
	public virtual void OnInteracting(Object param = null)
	{
		OnInteractingAction.Invoke();
	}

	/// <summary>
	/// On Interact Up, similar to on button up
	/// </summary>
	/// <param name="param"></param>
	public virtual void OnInteractUp(Object param = null)
	{
		IsOccupied = false;
		state = InteractableState.Idle;
		OnInteractUpAction.Invoke();
	}
}

public enum InteractableState
{
	Idle,
	Interacting,
}
