using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Step Instakill", menuName = "Ability Steps/Instakill")]
public class AbilityStep_InstaKill : AbilityStep_Base
{
	public override OnPlayResult OnPlay(params object[] parameters)
	{
		List<HealthEntityCombatObject> targets = parameters[0] as List<HealthEntityCombatObject>;

		OnPlayResult onPlayResult = new OnPlayResult();

		foreach (var t in targets)
		{
			t.InstaKill();
		}

		return onPlayResult;
	}
}