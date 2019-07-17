using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amiga.Inventory;

namespace Amiga.UI.Inventory
{
	public class InventoryPanel : MonoBehaviour, IDroppable, IRemovable
	{
		[Header("Prefabs")]
		[SerializeField]
		GameObject inventorySlotPrefab;

		[Header("References")]
		[SerializeField]
		RectTransform slotsHolder;
		[SerializeField]
		RectTransform imagesHolder;
		[SerializeField]
		RectTransform inventoryPanel;

		InventorySlot[,] slots;
		Dictionary<Vector2Int, InventorySlot> positionToSlotMap = new Dictionary<Vector2Int, InventorySlot>();

		Amiga.Inventory.Inventory Inventory
		{
			get
			{
				if (RunManager.Instance == null || RunManager.Instance.Inventory == null)
				{
					Debug.LogError("Inventory or Run Managers are null.");
					return null;
				}

				return RunManager.Instance.Inventory;
			}
		}


		void Start()
		{
			Populate();
		}

		public bool AddToInventory(Item item, InventorySlot targetSlot)
		{
			if (Inventory.AddToInventory(item, targetSlot.Position))
			{
				return true;
			}
			return false;
		}

		void RemoveFromInventory(InventorySlot slot)
		{
			Inventory.RemoveFromInventory(slot.Position);
			Populate();
		}

		public void RemoveFromHere(UISlot slot)
		{
			RemoveFromInventory(slot as InventorySlot);
		}

		public bool DropHere(Item item, UISlot targetSlot)
		{
			if (AddToInventory(item, targetSlot as InventorySlot))
			{
				Populate();
				return true;
			}

			return false;
		}

		public void Populate()
		{
			foreach (RectTransform slot in slotsHolder)
			{
				Destroy(slot.gameObject);
			}
			foreach (RectTransform image in imagesHolder)
			{
				Destroy(image.gameObject);
			}

			Vector2Int dim = Inventory.inventoryDimensions;
			float slotSize = Mathf.Min(inventoryPanel.sizeDelta.x / dim.y, inventoryPanel.sizeDelta.y / dim.x);

			slots = new InventorySlot[dim.x, dim.y];

			positionToSlotMap.Clear();

			for (int r = 0; r < dim.x; r++)
			{
				for (int c = 0; c < dim.y; c++)
				{
					InventorySlot newSlot = Instantiate(inventorySlotPrefab, slotsHolder).GetComponent<InventorySlot>();
					newSlot.Init(new Vector2Int(r, c), slotSize);
					
					((RectTransform)newSlot.transform).anchorMin = Vector2.up;
					((RectTransform)newSlot.transform).anchorMax = Vector2.up;
					((RectTransform)newSlot.transform).anchoredPosition = new Vector2(c * slotSize + slotSize / 2, - r * slotSize - slotSize / 2);
					((RectTransform)newSlot.transform).sizeDelta = new Vector2(slotSize, slotSize);

					Image newImage = new GameObject("Image " + r + ", " + c).AddComponent<Image>();
					newImage.transform.SetParent(imagesHolder);
					((RectTransform)newImage.transform).anchorMin = Vector2.up;
					((RectTransform)newImage.transform).anchorMax = Vector2.up;
					((RectTransform)newImage.transform).anchoredPosition = new Vector2(c * slotSize + slotSize / 2, - r * slotSize - slotSize / 2);
					((RectTransform)newImage.transform).sizeDelta = new Vector2(slotSize, slotSize);

					newSlot.SetImages(newImage);

					newSlot.SetItem(null);

					slots[r, c] = newSlot;
					newSlot.gameObject.GetComponentInChildren<DroppableArea>().droppable = this;
					newSlot.gameObject.GetComponentInChildren<RemovableArea>().removable = this;
					positionToSlotMap.Add(newSlot.Position, newSlot);
				}
			}

			Item[,] items = RunManager.Instance.Inventory.inventory;
			List<Item> exclusions = new List<Item>();

			for (int r = 0; r < dim.x; r++)
			{
				for (int c = 0; c < dim.y; c++)
				{
					Item item = items[r, c];

					if (item == null)
					{
						slots[r, c].SetItem(item);
						continue;
					}
					else if (exclusions.Contains(item))
					{
						continue;
					}

					exclusions.Add(item);
					InventorySlot slot = slots[r, c];

					slot.SetItem(item);

					if (item.Dimensions.x > 1 || item.Dimensions.y > 1)
					{
						for (int dr = 0; dr < item.Dimensions.x; dr++)
						{
							for (int dc = 0; dc < item.Dimensions.y; dc++)
							{
								if (dr == 0 && dc == 0)
									continue;

								slots[r + dr, c + dc].SetPartItem(item, slot);
							}
						}
					}
				}
			}
		}
	}
}