using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonTile
{
	public EncounterScriptableObject Encounter { get; private set; }
	public LevelGenerator.Room Room { get; private set; }
	public bool Visited { get; private set; }
	public bool Seen { get; private set; }

	public DungeonTile(LevelGenerator.Room room, EncounterScriptableObject encounter)
	{
		Encounter = encounter;
		Room = room;
		Visited = false;
		Seen = false;
	}

	public void TileVisited()
	{
		Visited = true;
	}

	public void TileSeen()
	{
		Seen = true;
	}
}