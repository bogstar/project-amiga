using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Step Draw Card", menuName = "Ability Steps/Draw Card Effect")]
public class AbilityStep_DrawCard : AbilityStep_Base
{
	public int amount;

	public override OnPlayResult OnPlay(params object[] parameters)
	{
		List<HealthEntityCombatObject> targets = parameters[0] as List<HealthEntityCombatObject>;

		OnPlayResult onPlayResult = new OnPlayResult();

		foreach (var t in targets)
		{
			((PlayerCombatObject)t).DrawCards(amount);
		}

		return onPlayResult;
	}
}