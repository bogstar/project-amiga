using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Step Revive", menuName = "Ability Steps/Revive")]
public class AbilityStep_Revive : AbilityStep_Base
{
	public GameObject particleEffectPrefab;

	public override OnPlayResult OnPlay(params object[] parameters)
	{
		List<HealthEntityCombatObject> targets = parameters[0] as List<HealthEntityCombatObject>;

		OnPlayResult onPlayResult = new OnPlayResult();

		foreach (var t in targets)
		{
			((PlayerCombatObject)t).Revive();
		}

		foreach (var t in targets)
		{
			GameObject particlePrefab = particleEffectPrefab;
			if (particleEffectPrefab == null)
			{
				particlePrefab = FindObjectOfType<CombatManager>().particlePrefab;
			}
			GameObject go = Instantiate(particlePrefab, t.transform);
			go.GetComponent<ParticlesEffect>().Init(0);
		}

		return onPlayResult;
	}
}