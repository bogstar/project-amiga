using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy : HealthEntity
{
	public AbilitiesWithChance[] abilities;

	public AudioClip[] attackSounds;
	public AudioClip[] deathSounds;

	public Enemy(EnemyScriptableObject data) : base(data)
	{
		if (data.fluctuateStartingHealth)
		{
			FluctuateMaxHealth(data.startingHealthFluctuationAmount);
		}

		Health = MaxHealth;
		abilities = data.abilities;
		attackSounds = data.attackSounds;
		deathSounds = data.deathSounds;
		alignment = Alignment.Evil;
	}

	/// <summary>
	/// Set max health to amount between 1 +- fluctuation amount.
	/// </summary>
	void FluctuateMaxHealth(float fluctuationAmount)
	{
		float startingHealthFluctuationAmount = Mathf.Clamp01(fluctuationAmount);
		MaxHealth = (int)(Random.Range(1 - startingHealthFluctuationAmount, 1 + startingHealthFluctuationAmount) * MaxHealth);
	}

	[System.Serializable]
	public struct AbilitiesWithChance
	{
		public EnemyAbilityScriptableObject ability;
		public int chance;
	}
}