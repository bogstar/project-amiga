using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Step Damage", menuName = "Ability Steps/Damage")]
public class AbilityStep_Damage : AbilityStep_Base
{
	public DamageType damageType;
	public DamageTypePerc damageTypePerc;
	public int amount;
	public bool canCriticallyStrike;
	public GameObject particleEffectPrefab;

	public enum DamageType { None, Physical, Magic, True }
	public enum DamageTypePerc { None, Flat, PercentageMaxHP }

	/// <summary>
	/// Takes 2 parameters: List<HealthEntityCombatObject> targets, HealthEntityCombatObject attacker.
	/// </summary>
	/// <param name="parameters"></param>
	/// <returns></returns>
	public override OnPlayResult OnPlay(params object[] parameters)
	{
		List<HealthEntityCombatObject> targets = parameters[0] as List<HealthEntityCombatObject>;
		HealthEntityCombatObject attacker = parameters[1] as HealthEntityCombatObject;

		OnPlayResult onPlayResult = new OnPlayResult();

		foreach (var t in targets)
		{
			HealthEntityCombatObject.DamageInfo damageInfo = t.TakeDamage(Instantiate<AbilityStep_Damage>(this), attacker);
			onPlayResult.unshieldedDamage = damageInfo.damageDealt;
		}

		foreach (var t in targets)
		{
			GameObject particlePrefab = particleEffectPrefab;
			if(particleEffectPrefab == null)
			{
				particlePrefab = FindObjectOfType<CombatManager>().particlePrefab;
			}
			GameObject go = Instantiate(particlePrefab, t.transform);
			go.GetComponent<ParticlesEffect>().Init(0);
		}

		return onPlayResult;
	}

	public override string GetValueForIndex(int index)
	{
		switch (index)
		{
			case 0:
				return amount.ToString();
			case 1:
				switch (damageType)
				{
					case DamageType.Magic:
						return "magic";
					case DamageType.Physical:
						return "physical";
					case DamageType.True:
						return "true";
					default:
						return null;
				}
			default:
				return null;
		}
	}
}