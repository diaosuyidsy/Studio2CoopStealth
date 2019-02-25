using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class CompareFloat : Conditional
{
	public SharedFloat variable;
	public bool biggerThan = true;
	public SharedFloat compareTo;

	public override TaskStatus OnUpdate()
	{
		if (biggerThan)
		{
			return variable.Value >= compareTo.Value ? TaskStatus.Success : TaskStatus.Failure;
		}
		else
		{
			return variable.Value <= compareTo.Value ? TaskStatus.Success : TaskStatus.Failure;
		}
	}

	public override void OnReset()
	{
		variable = 0;
		compareTo = 0;
	}
}
