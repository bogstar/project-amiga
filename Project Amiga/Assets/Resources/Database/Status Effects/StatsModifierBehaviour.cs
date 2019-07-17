using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Modifier Effect", menuName = "Status Effects/Behaviours/Stat Modifier")]
public class StatsModifierBehaviour : StatusEffectBehaviour
{
	public float amount;
	public HealthEntity.Stats.StatType stat;

	public override void OnAfflicted(params object[] parameters)
	{
		HealthEntityCombatObject afflictee = parameters[0] as HealthEntityCombatObject;

		afflictee.Stats.ModifyStat(stat, amount);
	}

	public override void OnUnafflicted(params object[] parameters)
	{
		HealthEntityCombatObject afflictee = parameters[0] as HealthEntityCombatObject;

		afflictee.Stats.UnmodifyStat(stat, amount);
	}

	public override void OnAttacked(params object[] parameters)
	{

	}

	public override void TickPerTurn(params object[] parameters)
	{
		
	}
}