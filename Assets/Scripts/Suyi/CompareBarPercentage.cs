using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CompareBarPercentage : Conditional
{
	public SharedGameObject Player;
	public SharedFloat BarAmount;
	public SharedFloat Percentage;

	public override TaskStatus OnUpdate()
	{
		Vector3 newSelfPos = new Vector3(transform.position.x, 0f, transform.position.z);
		Vector3 newPlayerPos = new Vector3(Player.Value.transform.position.x, 0f, Player.Value.transform.position.z);
		float distanceToPlayer = Vector3.Distance(newSelfPos, newPlayerPos);

		return BarAmount.Value > distanceToPlayer * Percentage.Value ? TaskStatus.Success : TaskStatus.Failure;
	}
}