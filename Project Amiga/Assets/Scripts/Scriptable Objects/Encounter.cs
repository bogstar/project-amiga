using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Encounter
{
	public EnemyScriptableObject[] enemies;
	public Amiga.Inventory.ItemScriptableObject[] drops;

	public Encounter(EncounterScriptableObject encounterData)
	{
		enemies = encounterData.enemies;
		drops = encounterData.drops;
	}
}