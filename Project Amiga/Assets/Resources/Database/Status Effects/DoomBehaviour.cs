using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Doom Effect", menuName = "Status Effects/Behaviours/Doom")]
public class DoomBehaviour : StatusEffectBehaviour
{
	public AbilityStep_Base[] actions;

	public override void TickPerTurn(params object[] parameters)
	{

	}

	public override void OnAfflicted(params object[] paramters)
	{
		
	}

	public override void OnAttacked(params object[] parameters)
	{
		
	}

	public override void OnUnafflicted(params object[] paramters)
	{
		List<HealthEntityCombatObject> targets = paramters[0] as List<HealthEntityCombatObject>;

		foreach (var a in actions)
		{
			a.OnPlay(targets);
		}
	}
}