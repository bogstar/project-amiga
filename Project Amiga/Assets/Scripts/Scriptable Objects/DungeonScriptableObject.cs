using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon", menuName = "Dungeon/Dungeon")]
public class DungeonScriptableObject : ScriptableObject
{
	public new string name;

	public EncounterScriptableObject[] encounters;

	[Range(5, 100)]
	public int numberOfTiles;
	[Range(2, 100)]
	public int frequencyOfHallwayDiversions;
	[Range(2, 100)]
	public int lengthOfHallways;
}