using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amiga.Inventory;

public class RunManager : Singleton<RunManager>
{
	public int Gold { get; private set; }

	public DungeonScriptableObject CurrentDungeon { get; private set; }

	public List<Equipment> equipmentInventory;

	public List<DungeonTile> Tiles { get; private set; }

	public DungeonTile currentTile;

	public Player player1;
	public Player player2;

	public Amiga.Inventory.Inventory Inventory { get; private set; }

	System.Action OnEndRun;
	public System.Action OnGoldChange;

	EncounterScriptableObject currentEncounter;


	public void GenerateLevel()
	{
		int unixTimestamp = ((long)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds).GetHashCode();

		LevelGenerator levelGenerator = new LevelGenerator(CurrentDungeon.numberOfTiles, 0.9f, unixTimestamp, CurrentDungeon.frequencyOfHallwayDiversions, CurrentDungeon.lengthOfHallways);
		levelGenerator.GenerateLevel();

		foreach (var room in levelGenerator.AllRooms)
		{
			EncounterScriptableObject encounter = null;

			if (!(room.Position.x == 0 && room.Position.y == 0))
			{
				encounter = (Random.Range(0, 2) == 1) ? CurrentDungeon.encounters[Random.Range(0, CurrentDungeon.encounters.Length)] : null;
			}

			DungeonTile dt = new DungeonTile(room, encounter);
			
			if (room.Position.x == 0 && room.Position.y == 0)
			{
				currentTile = dt;
				currentTile.TileVisited();
				currentTile.TileSeen();
			}

			dt.TileSeen();

			Tiles.Add(dt);
		}
	}

	public void TileVisited(DungeonTile tile)
	{
		tile.TileVisited();
		currentTile = tile;
	}

	public void StartCombat(EncounterScriptableObject encounter)
	{
		currentEncounter = encounter;
		GameManager.Instance.ChangeState(GameManager.State.Combat, OnCombatSceneLoaded);
	}

	void OnCombatSceneLoaded()
	{
		List<Player> players = new List<Player> { player1, player2 };
		FindObjectOfType<CombatManager>().StartCombat(currentEncounter, players, OnCombatEnd);
	}

	void OnCombatEnd(bool defeat = false)
	{
		if (defeat)
		{
			EndRun();
		}
		else
		{
			currentEncounter = null;
			GameManager.Instance.ChangeState(GameManager.State.Dungeon);
		}
	}

	public void StartNewRun(DungeonScriptableObject dungeon, System.Action OnEndRunCallback)
	{
		Inventory = gameObject.AddComponent<Amiga.Inventory.Inventory>();

		player1 = new Player(SessionManager.Instance.unlockedCharacters[0]);
		player2 = new Player(SessionManager.Instance.unlockedCharacters[1]);
		Gold = 0;

		Tiles = new List<DungeonTile>();
		CurrentDungeon = dungeon;
		OnEndRun = OnEndRunCallback;
		GenerateLevel();
	}

	public void EquipForPlayer(Player player, Equipment equipment)
	{
		UnequipForPlayer(player, equipment.slot);
		if (equipmentInventory.Contains(equipment))
		{
			equipmentInventory.Remove(equipment);
			player.EquipEquipment(equipment);
		}
	}

	public void UnequipForPlayer(Player player, Equipment.Slot type, int trinketIndex = -1)
	{
		Equipment e = player.UnequipEquipment(type, trinketIndex);
		if (e != null)
		{
			equipmentInventory.Add(e);
		}
	}

	public List<Equipment> GetAllEquipmentByType(Equipment.Slot type)
	{
		List<Equipment> equipment = new List<Equipment>();
		foreach (var e in equipmentInventory)
		{
			if (e.slot == type)
			{
				equipment.Add(e);
			}
		}
		return equipment;
	}

	public void EndRun()
	{
		OnEndRun();
		GameManager.Instance.ChangeState(GameManager.State.Town);
		GameObject.Destroy(gameObject);
	}
}