using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Empower Effect", menuName = "Status Effects/Behaviours/Empower")]
public class EmpowerBehaviour : StatusEffectBehaviour
{
	public bool immuneToDamage;
	public bool immuneToDebuffs;

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