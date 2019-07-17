using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Step Afflict Satus Effect", menuName = "Ability Steps/Afflict Status Effect")]
public class AbilityStep_AfflictStatusEffect : AbilityStep_Base
{
	public StatusEffectScriptableObject statusEffect;
	public int duration;

	public override OnPlayResult OnPlay(params object[] parameters)
	{
		List<HealthEntityCombatObject> targets = parameters[0] as List<HealthEntityCombatObject>;

		OnPlayResult onPlayResult = new OnPlayResult();

		foreach (var t in targets)
		{
			t.AfflictWithStatusEffect(statusEffect, duration);
		}

		onPlayResult.effectApplied = statusEffect;

		return onPlayResult;
	}

	public override string GetValueForIndex(int index)
	{
		switch (index)
		{
			case 0:
				return duration.ToString();
			case 1:
				return (duration > 1) ? "s" : "";
			default:
				return null;
		}
	}
}