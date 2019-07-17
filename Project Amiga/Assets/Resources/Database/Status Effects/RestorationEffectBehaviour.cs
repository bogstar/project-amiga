using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Restoration Effect", menuName = "Status Effects/Behaviours/Restoration")]
public class RestorationEffectBehaviour : StatusEffectBehaviour
{
	public int amount;
	public AbilityStep_Restore.Type type;

	public override void OnAfflicted(params object[] paramters)
	{
		
	}

	public override void OnUnafflicted(params object[] paramters)
	{

	}

	public override void OnAttacked(params object[] parameters)
	{
		
	}

	public override void TickPerTurn(params object[] parameters)
	{
		HealthEntityCombatObject target = parameters[0] as HealthEntityCombatObject;

		switch (type)
		{
			case AbilityStep_Restore.Type.Mana:
				((PlayerCombatObject)target).ModifyMana(amount);
				break;
		}
	}
}