using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageCalculator
{
    public static ClashResult CaclulateFlatDamage(AbilityStep_Damage step, HealthEntityCombatObject attacker, HealthEntityCombatObject defender)
	{
		ClashResult result = new ClashResult();
		
		// If defender is dead, result fails.
		if (step == null || defender == null || defender.IsDead)
		{
			result.name = ClashResult.Name.Failed;
			return result;
		}

		// If defender is immune to damage, result immune.
		if (defender.IsImmuneToDamage())
		{
			result.name = ClashResult.Name.Immune;
			return result;
		}

		// Grab the damage amount.
		result.damage = step.amount;

		if (attacker != null)
		{
			// Multiply with raw damage of the attacker.
			result.damage = (int)(result.damage * attacker.Stats.rawDamageMultiplier.Value);

			// Check for vulnerable
			if (defender.IsVulnerable())
			{
				result.damage = (int)(result.damage * 1.5f);
			}

			// Crit calculator.
			if (step.canCriticallyStrike)
			{
				float attackerCritChance = attacker.Stats.criticalChance.Value;

				// Get negative crit multiplier on target.
				foreach (var behaviour in defender.GetCritMultiplierBehaviours())
				{
					attackerCritChance *= behaviour.modifyCritChanceAgainst;
				}

				// Roll the crit and double the damage.
				float critValue = Random.Range(0f, 1f);

				// Critical is success.
				if (critValue < attackerCritChance)
				{
					result.damage *= 2;
					result.criticalHit = true;
				}
			}

			// Calculate damage against armor or magic resist.
			switch (step.damageType)
			{
				case AbilityStep_Damage.DamageType.None:
					result.damage = 0;
					break;
				case AbilityStep_Damage.DamageType.Physical:
					// defender armor - attacker armor pen => clamped between 0-1.
					float defenderPenetratedArmor = Mathf.Clamp01(defender.Stats.armor.Value - attacker.Stats.armorPenetration.Value);
					result.damage = (int)(result.damage * (1 - defenderPenetratedArmor));
					break;
				case AbilityStep_Damage.DamageType.Magic:
					// defender MR - attacker mr pen => clamped between 0-1.
					float defenderPenetratedMR = Mathf.Clamp01(defender.Stats.magicResist.Value - attacker.Stats.magicPenetration.Value);
					result.damage = (int)(result.damage * (1 - defenderPenetratedMR));
					break;
			}
		}

		// Clamp it to 0.
		result.damage = Mathf.Max(0, result.damage);

		// If the clamped damage is 0, result immune.
		if (result.damage < 1)
		{
			result.name = ClashResult.Name.Immune;
			return result;
		}

		result.name = ClashResult.Name.Damage;
		return result;
	}

	public struct ClashResult
	{
		public Name name;
		public int damage;
		public bool criticalHit;

		public enum Name
		{
			Failed,
			Immune,
			Damage
		}
	}
}