using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Enemy")]
public class EnemyScriptableObject : HealthEntityScriptableObject
{
	public bool fluctuateStartingHealth;
	[Range(0f, 1f)]
	public float startingHealthFluctuationAmount;
	public AudioClip[] attackSounds;
	public AudioClip[] deathSounds;

	public Enemy.AbilitiesWithChance[] abilities;
}