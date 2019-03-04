using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
	public string ButtonName;
	public float BaseCoolDown = 1f;

	protected float coolDownTimeLeft = 0f;
	protected float nextReadyTime;

	/// <summary>
	/// Pressed Down the Button
	/// </summary>
	public abstract void OnPressedDownAbility();

	/// <summary>
	/// Holding the button
	/// </summary>
	public virtual void OnHoldAbility() { }

	/// <summary>
	/// Lift Up the Button
	/// </summary>
	public virtual void OnLiftUpAbility() { }

	/// <summary>
	/// Cool Down Should run in Update
	/// </summary>
	public virtual void CoolDown()
	{
		coolDownTimeLeft -= Time.deltaTime;
	}

}
