using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Thorns Effect", menuName = "Status Effects/Behaviours/Thorns")]
public class ThornsBehaviour : StatusEffectBehaviour
{
	public int amount;

	public override void OnAfflicted(params object[] paramters)
	{

	}

	public override void OnUnafflicted(params object[] paramters)
	{

	}

	public override void OnAttacked(params object[] parameters)
	{
		HealthEntityCombatObject attacker = parameters[0] as HealthEntityCombatObject;

		AbilityStep_Damage dam = new AbilityStep_Damage();
		dam.amount = amount;
		dam.damageType = AbilityStep_Damage.DamageType.True;
		dam.damageTypePerc = AbilityStep_Damage.DamageTypePerc.Flat;
		attacker.TakeDamage(dam, null);
	}

	public override void TickPerTurn(params object[] parameters)
	{

	}
}