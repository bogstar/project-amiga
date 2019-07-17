using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weaken Effect", menuName = "Status Effects/Behaviours/Weaken")]
public class WeakenBehaviour : StatusEffectBehaviour
{
	public float modifyCritChanceAgainst;

	public override void OnAfflicted(params object[] parameters)
	{
	}

	public override void OnUnafflicted(params object[] parameters)
	{
	}

	public override void OnAttacked(params object[] parameters)
	{
	}

	public override void TickPerTurn(params object[] parameters)
	{
	}
}