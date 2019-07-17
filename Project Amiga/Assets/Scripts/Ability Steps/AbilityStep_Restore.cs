using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Step Restore", menuName = "Ability Steps/Restore")]
public class AbilityStep_Restore : AbilityStep_Base
{
	public Type type;
	public int amount;
	public GameObject particleEffectPrefab;

	public enum Type { None, Health, Block, Mana }

	public override OnPlayResult OnPlay(params object[] parameters)
	{
		List<HealthEntityCombatObject> targets = parameters[0] as List<HealthEntityCombatObject>;
		//HealthEntity healer = parameters[1] as HealthEntity;

		OnPlayResult onPlayResult = new OnPlayResult();

		if(type == Type.Health)
		{
			foreach (var t in targets)
			{
				t.TakeHealing(amount);
			}
		}
		else if(type == Type.Block)
		{
			foreach (var t in targets)
			{
				t.ModifyBlock(amount);
			}
		}
		else if(type == Type.Mana)
		{
			foreach (var t in targets)
			{
				if(t is PlayerCombatObject)
				{
					((PlayerCombatObject)t).ModifyMana(amount);
				}
			}
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
}