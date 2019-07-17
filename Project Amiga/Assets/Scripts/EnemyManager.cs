using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EnemyManager
{
	public GameObject entityPrefab;
	public GameObject enemyPrefab;
	public Transform enemyPanel;

	public Sprite damagingAbilitySprite;
	public Sprite blockingAbilitySprite;
	public Sprite healingAbilitySprite;
	public Sprite buffingAbilitySprite;
	public Sprite debuffingAbilitySprite;

	public List<EnemyCombatObject> SpawnEnemies(List<EnemyCombatObject> enemiesSO, System.Action<HealthEntityCombatObject> OnEnemyDeathCallback)
	{
		List<EnemyCombatObject> enemies = new List<EnemyCombatObject>();

		for (int i = 0; i < enemiesSO.Count; i++)
		{
			EnemyCombatObject enemy = enemiesSO[i];
			enemy.gameObject.name = "Enemy " + i;
			enemies.Add(enemy);
		}

		return enemies;
	}

	public void KillAllEnemies(List<EnemyCombatObject> enemies)
	{
		for (int i = enemies.Count-1; i > -1; i--)
		{
			enemies[i].InstaKill();
		}
	}

	public void SetEnemiesTargetable(bool targetable, List<EnemyCombatObject> enemies)
	{
		foreach(var enemy in enemies)
		{
			enemy.AllowTargetable(targetable);
		}
	}
}