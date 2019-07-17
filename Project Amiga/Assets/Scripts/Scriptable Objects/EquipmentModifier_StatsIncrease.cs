using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Modifier", menuName = "Characters/Equipment/Modifiers/Stat Modifier", order = 11)]
public class EquipmentModifier_StatsIncrease : EquipmentModifierSO
{
	public StatModifier[] modifiers;

	public override void OnEquip(params object[] parameters)
	{
		Player equipee = parameters[0] as Player;
		foreach (var modifier in modifiers)
		{
			if (modifier.stat == HealthEntity.Stats.StatType.MaxMana)
			{
				equipee.mana += (int)modifier.value;
			}
			else if (modifier.stat == HealthEntity.Stats.StatType.MaxHealth)
			{
				equipee.MaxHealth += (int)modifier.value;
				equipee.Health = Mathf.Clamp(equipee.Health, 0, equipee.MaxHealth);
			}
			else
			{
				equipee.stats.ModifyStat(modifier.stat, modifier.value);
			}
		}
	}

	public override void OnUnequip(params object[] parameters)
	{
		Player equipee = parameters[0] as Player;
		foreach (var modifier in modifiers)
		{
			if (modifier.stat == HealthEntity.Stats.StatType.MaxMana)
			{
				equipee.mana -= (int)modifier.value;
			}
			else if (modifier.stat == HealthEntity.Stats.StatType.MaxHealth)
			{
				equipee.MaxHealth -= (int)modifier.value;
				equipee.Health = Mathf.Clamp(equipee.Health, 0, equipee.MaxHealth);
			}
			else
			{
				equipee.stats.UnmodifyStat(modifier.stat, modifier.value);
			}
		}
	}

	[System.Serializable]
	public struct StatModifier
	{
		public HealthEntity.Stats.StatType stat;
		public float value;
	}
}