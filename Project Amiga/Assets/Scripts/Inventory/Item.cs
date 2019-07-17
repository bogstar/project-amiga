using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amiga.Inventory
{
	[System.Serializable]
	public class Item
	{
		public string Name { get; protected set; }
		public string Description { get; protected set; }
		public Sprite Image { get; protected set; }
		public Vector2Int Dimensions { get; protected set; }

		public void SwapDimensions()
		{
			Dimensions = new Vector2Int(Dimensions.y, Dimensions.x);
		}

		public Item(ItemScriptableObject data)
		{
			if (data.name == "")
			{
				Debug.LogWarning("Name for item: " + data + " is empty.");
				Name = data.ToString();
			}
			else
			{
				Name = data.name;
			}

			if (data.description == "")
			{
				Debug.LogWarning("Description for item: " + data + " is empty.");
			}
			else
			{
				Description = data.description;
			}

			if (data.image == null)
			{
				Debug.LogWarning("Image for item: " + data + " is null.");
				// TODO: Set some kind of default image.
				Debug.LogError("Set some kind of default image.");
				Image = null;
			}
			else
			{
				Image = data.image;
			}

			if (data.dimensions.x < 1 || data.dimensions.y < 1)
			{
				Debug.LogWarning("Dimensions for item: " + data + " are less than 1.");
				Dimensions = new Vector2Int(Mathf.Max(1, data.dimensions.x), Mathf.Max(1, data.dimensions.y));
			}
			else
			{
				Dimensions = data.dimensions;
			}
		}

		public static Item CreateItem(ItemScriptableObject item)
		{
			if (item is EquipmentScriptableObject)
			{
				Equipment e = new Equipment(item as EquipmentScriptableObject);
				return e;
			}

			Item i = new Item(item);
			return i;
		}
	}
}