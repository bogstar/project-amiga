using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class HealthEntity
{
	public string Name { get; private set; }

	public GameObject Model { get; private set; }

	public int Health { get; set; }
	public int MaxHealth { get; set; }
	public Stats stats;

	public List<StatusEffect> statusEffects = new List<StatusEffect>();
	public Alignment alignment;

	protected HealthEntity(HealthEntityScriptableObject data)
	{
		Name = data.name;
		Model = data.model;
		Health = MaxHealth = data.health;
		stats = new Stats(data.criticalChance, data.armor, data.magicResist, data.armorPenetration, data.magicPenetration, data.rawDamageMultiplier);
	}

	public enum Alignment { None, Good, Evil }

	[System.Serializable]
	public struct Stats
	{
		public StatFloat criticalChance;
		public StatFloat armor;
		public StatFloat magicResist;
		public StatFloat armorPenetration;
		public StatFloat magicPenetration;
		public StatFloat rawDamageMultiplier;

		public Stats(float criticalChance, float armor, float magicResist, float armorPenetration, float magicPenetration, float rawDamageMultiplier)
		{
			this.criticalChance = new StatFloat(criticalChance);
			this.armor = new StatFloat(armor);
			this.magicResist = new StatFloat(magicResist);
			this.armorPenetration = new StatFloat(armorPenetration);
			this.magicPenetration = new StatFloat(magicPenetration);
			this.rawDamageMultiplier = new StatFloat(rawDamageMultiplier);
		}

		public void ModifyStat(StatType type, float value)
		{
			switch (type)
			{
				case StatType.Armor:
					armor.ModifyValue(value);
					break;
				case StatType.ArmorPenetration:
					armorPenetration.ModifyValue(value);
					break;
				case StatType.CriticalChance:
					criticalChance.ModifyValue(value);
					break;
				case StatType.MagicPenetration:
					magicPenetration.ModifyValue(value);
					break;
				case StatType.MagicResist:
					magicResist.ModifyValue(value);
					break;
				case StatType.RawDamageMultiplier:
					rawDamageMultiplier.ModifyValue(value);
					break;
			}
		}

		public void UnmodifyStat(StatType type, float value)
		{
			switch (type)
			{
				case StatType.Armor:
					armor.UnmodifyValue(value);
					break;
				case StatType.ArmorPenetration:
					armorPenetration.UnmodifyValue(value);
					break;
				case StatType.CriticalChance:
					criticalChance.UnmodifyValue(value);
					break;
				case StatType.MagicPenetration:
					magicPenetration.UnmodifyValue(value);
					break;
				case StatType.MagicResist:
					magicResist.UnmodifyValue(value);
					break;
				case StatType.RawDamageMultiplier:
					rawDamageMultiplier.UnmodifyValue(value);
					break;
			}
		}

		public enum StatType { None, CriticalChance, Armor, MagicResist, ArmorPenetration, MagicPenetration, RawDamageMultiplier, MaxMana, MaxHealth }
	}

	[System.Serializable]
	public struct StatFloat
	{
		float baseValue;
		List<float> modifiers;
		public float Value
		{
			get
			{
				float finalValue = baseValue;
				if (modifiers == null)
				{
					return finalValue;
				}
				foreach (var modifier in modifiers)
				{
					finalValue += modifier;
				}
				return finalValue;
			}
		}

		public StatFloat(float startValue)
		{
			this.modifiers = new List<float>();
			this.baseValue = startValue;
		}

		public void ModifyValue(float modifier)
		{
			modifiers.Add(modifier);
		}

		public void UnmodifyValue(float modifier)
		{
			if (modifiers.Contains(modifier))
			{
				modifiers.Remove(modifier);
			}
		}
	}

	public struct DamageInfo
	{
		public int damageDealt;
	}
}