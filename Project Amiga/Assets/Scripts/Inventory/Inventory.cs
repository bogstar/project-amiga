using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amiga.Inventory
{
	public class Inventory : MonoBehaviour
	{
		public Vector2Int inventoryDimensions { get; private set; }

		public System.Action OnInventoryUpdate;

		public Item[,] inventory { get; private set; }
		Dictionary<Item, List<Vector2Int>> itemToLocationMap = new Dictionary<Item, List<Vector2Int>>(); 

		void Start()
		{
			inventoryDimensions = new Vector2Int(4, 12);

			inventory = new Item[inventoryDimensions.x, inventoryDimensions.y];

			for (int r = 0; r < inventory.GetLength(0); r++)
			{
				for (int c = 0; c < inventory.GetLength(1); c++)
				{
					inventory[r, c] = null;
				}
			}

			AddToInventory(RunManager.Instance.CurrentDungeon.encounters[0].drops[1]);
		}

		List<Vector2Int> GetAvailableLocations(Vector2Int dimensions, Vector2Int location)
		{
			List<Vector2Int> possibilites = new List<Vector2Int>();

			Vector2Int position = location;
			Vector2Int itemDim = dimensions;

			bool sprawlRight = true;
			bool sprawlDown = true;

			if ((position.x + (itemDim.x - 1)) > inventoryDimensions.x - 1)
				sprawlDown = false;

			if ((position.y + (itemDim.y - 1)) > inventoryDimensions.y - 1)
				sprawlRight = false;

			int startingR = !sprawlDown ? inventoryDimensions.x - itemDim.x : position.x;
			int startingC = !sprawlRight ? inventoryDimensions.y - itemDim.y : position.y;
			int endingR = !sprawlDown ? inventoryDimensions.x : position.x + itemDim.x;
			int endingC = !sprawlRight ? inventoryDimensions.y : position.y + itemDim.y;

			for (int r = startingR; r < endingR; r++)
			{
				for (int c = startingC; c < endingC; c++)
				{
					possibilites.Add(new Vector2Int(r, c));
				}
			}

			foreach (var p in possibilites)
			{
				if(!LocationEmpty(p))
				{
					return new List<Vector2Int>();
				}
			}

			return possibilites;
		}

		bool LocationEmpty(Vector2Int location)
		{
			if (inventory[location.x, location.y] == null)
			{
				return true;
			}

			return false;
		}

		bool LocationExists(Vector2Int location)
		{
			if (location.x > (inventoryDimensions.x - 1) || location.x < 0 || location.y > (inventoryDimensions.y - 1) || location.y < 0)
			{
				return false;
			}

			return true;
		}

		Vector2Int? GetFreeLocation(Vector2Int dimensions)
		{
			for (int r = 0; r < inventoryDimensions.x; r++)
			{
				for (int c = 0; c < inventoryDimensions.y; c++)
				{
					if (GetAvailableLocations(dimensions, new Vector2Int(r, c)).Count != 0)
					{
						return new Vector2Int(r, c);
					}
				}
			}

			return null;
		}

		public bool AddToInventory(Item item)
		{
			Vector2Int? location = GetFreeLocation(item.Dimensions);

			if (location.HasValue)
			{
				AddToInventory(item, location.Value);
				return true;
			}

			return false;
		}

		public bool AddToInventory(ItemScriptableObject itemData)
		{
			Item newItem = Item.CreateItem(itemData);
			return AddToInventory(newItem);
		}

		public bool AddToInventory(Item item, Vector2Int location)
		{
			if (item == null)
			{
				Debug.LogError("Item is null.");
			}

			List<Vector2Int> locations = GetAvailableLocations(item.Dimensions, location);

			if (locations.Count < 1)
			{
				return false;
			}

			Vector2Int dimensions = item.Dimensions;

			foreach (var loc in locations)
			{
				inventory[loc.x, loc.y] = item;
			}

			if (itemToLocationMap.ContainsKey(item))
			{
				RemoveFromInventory(item);
			}
			itemToLocationMap.Add(item, locations);

			if (OnInventoryUpdate != null)
			{
				OnInventoryUpdate();
			}

			return true;
		}

		public bool AddToInventory(ItemScriptableObject itemData, Vector2Int location)
		{
			Item newItem = Item.CreateItem(itemData);
			return AddToInventory(newItem, location);
		}

		public void RemoveFromInventory(Vector2Int location)
		{
			if (!LocationExists(location))
			{
				return;
			}
			if (LocationEmpty(location))
			{
				return;
			}

			RemoveFromInventory(inventory[location.x, location.y]);
		}

		public void RemoveFromInventory(Item item)
		{
			if (!itemToLocationMap.ContainsKey(item))
			{
				return;
			}

			List<Vector2Int> locations = itemToLocationMap[item];

			foreach (var loc in locations)
			{
				inventory[loc.x, loc.y] = null;
			}

			itemToLocationMap.Remove(item);
			if (OnInventoryUpdate != null)
			{
				OnInventoryUpdate();
			}
		}

		public Vector2Int[] GetPositionsForItem(Item item)
		{
			List<Vector2Int> positions = new List<Vector2Int>();

			positions = itemToLocationMap[item];

			return positions.ToArray();
		}
	}
}