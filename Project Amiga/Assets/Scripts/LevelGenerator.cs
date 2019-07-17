using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator
{
	[Range(5, 100)]
	public int numberOfTiles;
	[Range(0, 1)]
	public float scale = 1;
	public string tileHolderName = "Room Holder";
	public int seed;
	[Range(2, 100)]
	public int frequencyOfHallwayDiversions;
	[Range(2, 100)]
	public int lengthOfHallways;

	Room startingRoom;
	Queue<Hallway> hallways;
	public List<Room> AllRooms
	{
		get
		{
			List<Room> rooms = new List<Room>();
			foreach (var hallway in hallways)
			{
				foreach (var room in hallway.Rooms)
				{
					rooms.Add(room);
				}
			}
			rooms.Add(startingRoom);
			return rooms;
		}
	}
	int RoomCount
	{
		get
		{
			return AllRooms.Count;
		}
	}


	public LevelGenerator(int numberOfTiles, float scale, int seed, int frequencyOfHallwayDiversions, int lengthOfHallways)
	{
		this.numberOfTiles = numberOfTiles;
		this.scale = scale;
		this.seed = seed;
		this.frequencyOfHallwayDiversions = frequencyOfHallwayDiversions;
		this.lengthOfHallways = lengthOfHallways;
	}

	public void GenerateLevel()
	{
		System.Random prng = new System.Random(seed.GetHashCode());
		hallways = new Queue<Hallway>();

		startingRoom = new Room(new Vector2Int(0, 0), null);

		int numberOfStartingHallways = prng.Next(2, 5);
		List<Direction> directionsLeft = GetFreeNeighbourSpots(startingRoom, AllRooms);
		List<Direction> pickedDirections = new List<Direction>();
		for (int i = 0; i < numberOfStartingHallways; i++)
		{
			Direction pickedDir = directionsLeft[prng.Next(0, directionsLeft.Count)];
			pickedDirections.Add(pickedDir);
			directionsLeft.Remove(pickedDir);
		}

		foreach (var dir in pickedDirections)
		{
			Room newRoom = new Room(GetPositionFromDirection(startingRoom, dir), startingRoom);
			CreateHallway(newRoom, dir);
		}

		while (RoomCount < numberOfTiles)
		{
			Hallway hallway = hallways.Peek();

			if (hallway.RoomCount % 2 == 0)
			{
				if (prng.Next(1, frequencyOfHallwayDiversions) <= hallway.RoomCount)
				{
					List<Direction> secondaryDirs = GetSecondaryDirs(hallway.Direction);
					Direction secondaryDir = secondaryDirs[prng.Next(0, secondaryDirs.Count)];
					secondaryDirs.Remove(secondaryDir);

					if (IsValidRoomPlacement(hallway.LastRoom, secondaryDir, AllRooms))
					{
						Room newRoom = new Room(GetPositionFromDirection(hallway.LastRoom, secondaryDir), hallway.LastRoom);
						CreateHallway(newRoom, secondaryDir);
						hallways.Dequeue();
						hallways.Enqueue(hallway);
						continue;
					}
					else if ((IsValidRoomPlacement(hallway.LastRoom, secondaryDirs[0], AllRooms)))
					{
						Room newRoom = new Room(GetPositionFromDirection(hallway.LastRoom, secondaryDirs[0]), hallway.LastRoom);
						CreateHallway(newRoom, secondaryDirs[0]);
						hallways.Dequeue();
						hallways.Enqueue(hallway);
						continue;
					}
				}
			}

			if (prng.Next(1, lengthOfHallways) <= lengthOfHallways - hallway.RoomCount)
			{
				hallway.AddRoom(AllRooms);
			}

			hallways.Dequeue();
			hallways.Enqueue(hallway);
		}
	}

	public static List<Direction> GetSecondaryDirs(Direction dir)
	{
		switch (dir)
		{
			case Direction.West:
			case Direction.East:
				return new List<Direction> { Direction.North, Direction.South };
			case Direction.North:
			case Direction.South:
				return new List<Direction> { Direction.West, Direction.East };
		}
		return null;
	}

	public static bool IsValidRoomPlacement(Room connector, Direction direction, List<Room> allRooms)
	{
		Vector2Int position = GetPositionFromDirection(connector, direction);
		if (GetRoomByPosition(position, allRooms) == null)
		{
			return true;
		}

		return false;
	}

	public static Vector2Int GetPositionFromDirection(Room room, Direction direction)
	{
		switch (direction)
		{
			case Direction.North:
				return new Vector2Int(room.Position.x, room.Position.y + 1);
			case Direction.South:
				return new Vector2Int(room.Position.x, room.Position.y - 1);
			case Direction.East:
				return new Vector2Int(room.Position.x + 1, room.Position.y);
			case Direction.West:
				return new Vector2Int(room.Position.x - 1, room.Position.y);
		}
		return default(Vector2Int);
	}

	public static Room GetRoomByPosition(Vector2Int position, List<Room> allRooms)
	{
		foreach (var room in allRooms)
		{
			if(room.Position == position)
			{
				return room;
			}
		}
		return null;
	}

	public static List<Direction> GetFreeNeighbourSpots(Room room, List<Room> allRooms)
	{
		List<Direction> neighbours = new List<Direction>();

		if (GetNeighbour(Direction.East, room, allRooms) == null)
			neighbours.Add(Direction.East);
		if (GetNeighbour(Direction.West, room, allRooms) == null)
			neighbours.Add(Direction.West);
		if (GetNeighbour(Direction.North, room, allRooms) == null)
			neighbours.Add(Direction.North);
		if (GetNeighbour(Direction.South, room, allRooms) == null)
			neighbours.Add(Direction.South);

		return neighbours;
	}

	public static List<Room> GetAllNeighbours(Room room, List<Room> allRooms)
	{
		List<Room> neighbours = new List<Room>();

		if (GetNeighbour(Direction.East, room, allRooms) != null)
			neighbours.Add(room);
		if (GetNeighbour(Direction.West, room, allRooms) != null)
			neighbours.Add(room);
		if (GetNeighbour(Direction.North, room, allRooms) != null)
			neighbours.Add(room);
		if (GetNeighbour(Direction.South, room, allRooms) != null)
			neighbours.Add(room);

		return neighbours;
	}

	public static Room GetNeighbour(Direction direction, Room room, List<Room> allRooms)
	{
		Vector2Int neighbourPos = room.Position;
		switch (direction)
		{
			case Direction.North:
				neighbourPos.y += 1;
				break;
			case Direction.South:
				neighbourPos.y -= 1;
				break;
			case Direction.East:
				neighbourPos.x += 1;
				break;
			case Direction.West:
				neighbourPos.x -= 1;
				break;
			default:
				return null;
		}
		if(GetRoomByPosition(neighbourPos, allRooms) != null)
		{
			return GetRoomByPosition(neighbourPos, allRooms);
		}

		return null;
	}

	public static Direction GetDirectionBetweenNeighbours(Room first, Room second)
	{
		if(first.Neighbours.Contains(second) && second.Neighbours.Contains(first))
		{
			if (first.Position.x < second.Position.x)
				return Direction.West;
			if (first.Position.x > second.Position.x)
				return Direction.East;
			if (first.Position.y < second.Position.y)
				return Direction.South;
			if (first.Position.y > second.Position.y)
				return Direction.North;
		}
		return Direction.All;
	}

	Hallway CreateHallway(Room newRoom, Direction heading)
	{
		Hallway newHallway = new Hallway(newRoom, heading);
		hallways.Enqueue(newHallway);
		return newHallway;
	}

	public class Hallway
	{
		public Direction Direction { get; private set; }
		public List<Room> Rooms { get; private set; }
		public int RoomCount { get { return Rooms.Count; } }
		public Room LastRoom { get { return Rooms[Rooms.Count - 1]; } }

		public Hallway(Room startingRoom, Direction heading)
		{
			Rooms = new List<Room>();
			Direction = heading;
			Rooms.Add(startingRoom);
		}

		public bool AddRoom(List<Room> allRooms)
		{
			if (IsValidRoomPlacement(LastRoom, Direction, allRooms))
			{
				Vector2Int position = GetPositionFromDirection(LastRoom, Direction);
				Room newRoom = new Room(position, LastRoom);
				Rooms.Add(newRoom);
				return true;
			}
			else
			{
				Room obstructingRoom = GetRoomByPosition(GetPositionFromDirection(LastRoom, Direction), allRooms);
				obstructingRoom.AddNeighbour(LastRoom);
				LastRoom.AddNeighbour(obstructingRoom);
				return false;
			}
		}
	}

	public class Room
	{
		public Vector2Int Position { get; private set; }
		public List<Room> Neighbours { get; private set; }
		public int NeighbourCount { get { return Neighbours.Count; } }

		public void AddNeighbour(Room neighbour)
		{
			if (!Neighbours.Contains(neighbour))
			{
				Neighbours.Add(neighbour);
			}
		}

		public Room(Vector2Int position, Room connector)
		{
			Neighbours = new List<Room>();
			Position = position;
			if (connector != null)
			{
				Neighbours.Add(connector);
				connector.AddNeighbour(this);
			}
		}
	}

	public enum Direction
	{
		North, South, West, East, All
	}
}