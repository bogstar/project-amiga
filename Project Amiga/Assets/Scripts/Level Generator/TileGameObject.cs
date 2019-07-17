using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGameObject : MonoBehaviour
{
	[Header("References")]
	public GameObject northWall;
	public GameObject southWall;
	public GameObject westWall;
	public GameObject eastWall;

	public LevelGenerator.Room tile;

	public void DisplayWall(bool display, params LevelGenerator.Direction[] walls)
	{
		foreach (var wall in walls)
		{
			switch (wall)
			{
				case LevelGenerator.Direction.North:
					southWall.SetActive(display);
					break;
				case LevelGenerator.Direction.South:
					northWall.SetActive(display);
					break;
				case LevelGenerator.Direction.East:
					westWall.SetActive(display);
					break;
				case LevelGenerator.Direction.West:
					eastWall.SetActive(display);
					break;
				case LevelGenerator.Direction.All:
					westWall.SetActive(display);
					eastWall.SetActive(display);
					southWall.SetActive(display);
					northWall.SetActive(display);
					break;
			}
		}
	}
}