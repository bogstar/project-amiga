using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Step Drain", menuName = "Ability Steps/Drain")]
public class AbilityStep_Drain : AbilityStep_Base
{
	public GameObject particleEffectPrefab;

	public override OnPlayResult OnPlay(params object[] parameters)
	{
		List<HealthEntityCombatObject> targets = parameters[0] as List<HealthEntityCombatObject>;
		OnPlayResult onPlayResult = (OnPlayResult)parameters[1];

		foreach (var t in targets)
		{
			t.TakeHealing(onPlayResult.unshieldedDamage);

			GameObject particlePrefab = particleEffectPrefab;
			if (particleEffectPrefab == null)
			{
				particlePrefab = FindObjectOfType<CombatManager>().particlePrefab;
			}

			Instantiate(particlePrefab, t.transform);
		}

		return default(OnPlayResult);
	}
}