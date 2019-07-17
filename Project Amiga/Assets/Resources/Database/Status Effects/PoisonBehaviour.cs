using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Posion Effect", menuName = "Status Effects/Behaviours/Poison")]
public class PoisonBehaviour : StatusEffectBehaviour
{
	public int amount;

	public override void TickPerTurn(params object[] parameters)
	{
		HealthEntityCombatObject target = parameters[0] as HealthEntityCombatObject;

		AbilityStep_Damage dam = CreateInstance<AbilityStep_Damage>();
		dam.amount = amount;
		dam.damageType = AbilityStep_Damage.DamageType.True;
		dam.damageTypePerc = AbilityStep_Damage.DamageTypePerc.Flat;
		target.TakeDamage(dam, null);
	}

	public override void OnAfflicted(params object[] paramters)
	{
		
	}

	public override void OnAttacked(params object[] parameters)
	{
		
	}

	public override void OnUnafflicted(params object[] paramters)
	{
		
	}
}