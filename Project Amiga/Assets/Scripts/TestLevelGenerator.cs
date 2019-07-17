using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestLevelGenerator : MonoBehaviour
{
	public GameObject TilePrefab;
	public Transform levelHolder;
	string tileHolderName = "Tile Holder";
	[Range(5, 100)]
	public int numberOfTiles;
	[Range(2, 100)]
	public int frequencyOfHallwayDiversions;
	[Range(2, 100)]
	public int lengthOfHallways;
	[Range(0, 1)]
	public float scale;
	public int seed;

	public void GenerateMap()
	{
		if (levelHolder.Find(tileHolderName))
		{
			DestroyImmediate(levelHolder.Find(tileHolderName).gameObject);
		}

		GameObject tileHolder = new GameObject(tileHolderName);
		tileHolder.transform.SetParent(levelHolder);

		LevelGenerator lg = new LevelGenerator(numberOfTiles, scale, seed, frequencyOfHallwayDiversions, lengthOfHallways);
		lg.GenerateLevel();

		foreach (var room in lg.AllRooms)
		{
			GameObject newTileGO = Instantiate(TilePrefab, tileHolder.transform);
			TileGameObject newTile = newTileGO.GetComponent<TileGameObject>();
			newTile.transform.localPosition = new Vector3(room.Position.x, 0, room.Position.y);
			newTile.DisplayWall(true, LevelGenerator.Direction.All);
			newTile.tile = room;

			foreach (var n in newTile.tile.Neighbours)
			{
				if (room.Neighbours.Contains(n))
				{
					LevelGenerator.Direction dir = LevelGenerator.GetDirectionBetweenNeighbours(room, n);
					newTile.DisplayWall(false, dir);
				}
			}
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(TestLevelGenerator))]
	public class TestLevelEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			((TestLevelGenerator)target).GenerateMap();
		}
	}
#endif
}