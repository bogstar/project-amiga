using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amiga.Inventory;
using Amiga.UI.Inventory;

namespace Amiga.UI.Dungeon
{
	public class InventoryMenu : HUDPanel
	{
		//public PlayerCardsPanel playerCardsPanel;

	
		/*
		public void UnhighlightAll()
		{
			if (slots == null)
				return;

			foreach (var s in slots)
			{
				s.Highlight(false);
			}
		}

		public void HighlightInventory(InventorySlot slot)
		{
			Amiga.Inventory.Inventory inventory = RunManager.Instance.Inventory;

			foreach (var s in slots)
			{
				s.Highlight(false);
			}

			if (grabbedSlot == null)
			{
				if (slot.Item != null)
				{
					Vector2Int[] slotPositions = inventory.GetPositionsForItem(slot.Item);

					foreach (var pos in slotPositions)
					{
						positionToSlotMap[pos].Highlight(true);
					}
				}
				else
				{
					slot.Highlight(true);
				}
			}
			else
			{
				Vector2Int inventoryDimensions = inventory.inventoryDimensions;
				Vector2Int position = slot.Position;
				Vector2Int itemDim = grabbedSlot.Item.Dimensions;

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
						slots[r, c].Highlight(true);
					}
				}
			}
		}*/

			/*
		private void OnDestroy()
		{
			RunManager.Instance.Inventory.OnInventoryUpdate -= Display;
		}
		*/
		/*
		void Populate()
		{
			foreach (Transform t in slotsHolder)
			{
				Destroy(t.gameObject);
			}
			foreach (Transform t in imagesHolder)
			{
				Destroy(t.gameObject);
			}

			Vector2Int dim = new Vector2Int(RunManager.Instance.Inventory.inventory.GetLength(0), RunManager.Instance.Inventory.inventory.GetLength(1));

			slots = new InventorySlot[dim.x, dim.y];

			positionToSlotMap.Clear();

			for (int r = 0; r < dim.x; r++)
			{
				for (int c = 0; c < dim.y; c++)
				{
					InventorySlot newSlot = Instantiate(inventorySlotPrefab, slotsHolder).GetComponent<InventorySlot>();
					newSlot.Init(new Vector2Int(r, c), 50);
					((RectTransform)newSlot.transform).anchorMin = Vector2.up;
					((RectTransform)newSlot.transform).anchorMax = Vector2.up;
					((RectTransform)newSlot.transform).anchoredPosition = new Vector2Int(c * 50 + 25, -r * 50 - 25);

					Image newImage = new GameObject("Image " + r + ", " + c).AddComponent<Image>();
					newImage.transform.SetParent(imagesHolder);
					((RectTransform)newImage.transform).anchorMin = Vector2.up;
					((RectTransform)newImage.transform).anchorMax = Vector2.up;
					((RectTransform)newImage.transform).anchoredPosition = new Vector2Int(c * 50 + 25, -r * 50 - 25);

					newSlot.SetImages(newImage);

					newSlot.SetItem(null);

					slots[r, c] = newSlot;
					positionToSlotMap.Add(newSlot.Position, newSlot);
				}
			}
		}*/
		/*
		public void ShowCardsForPlayer(PlayerInfoPanel playerInfoPanel)
		{
			GameManager.Instance.CurrentSceneSettings.ShowPanel(playerCardsPanel);
			playerCardsPanel.Display(playerInfoPanel.Player);
		}
		*//*
		public void HideCards()
		{
			GameManager.Instance.CurrentSceneSettings.HideTopPanel();
		}

		UISlot grabbedSlot;*/
		/*
		public void ReleaseItem()
		{
			if (grabbedSlot != null)
			{
				PlaceItem(grabbedSlot);
			}

			if (player1.Player.Name != "")
			{
				player1.Refresh();
			}
			if (player2.Player.Name != "")
			{
				player2.Refresh();
			}
		}*/
		/*
		public void RotateGrabbedItem()
		{
			if(grabbedSlot != null)
			{
				grabbedSlot.Item.SwapDimensions();
			}
		}*/

		/*
		public void GrabItem(IGrabbable grabbable)
		{
			UISlot slot = grabbable.GetSlot();

			if (slot.Item == null)
			{
				return;
			}

			if (slot is EquipmentSlot)
			{
				grabbedSlot = slot;
				slot.GrabItem(true);
			}
			else if(slot is InventorySlot)
			{
				grabbedSlot = ((InventorySlot)slot).MainSlot;
				((InventorySlot)slot).MainSlot.GrabItem(true);
			}
			else
			{
				grabbedSlot = slot;
				slot.GrabItem(true);
			}
		}
		*/

		/*
		void PlaceItem(UISlot newSlot)
		{
			Item grabbedItem = grabbedSlot.Item;
			int grabbedTrinketSlot = -1;

			if (grabbedSlot is InventorySlot)
			{
				RunManager.Instance.Inventory.RemoveFromInventory(grabbedItem);
			}
			else if(grabbedSlot is EquipmentSlot)
			{
				grabbedTrinketSlot = ((EquipmentSlot)grabbedSlot).TrinketIndex;
				((EquipmentSlot)grabbedSlot).Player.UnequipEquipment(((Equipment)grabbedItem).slot, grabbedTrinketSlot);
			}
			else
			{
				dropArea.RemoveFromDrop(grabbedItem);
			}

			if (newSlot is InventorySlot)
			{
				if (!RunManager.Instance.Inventory.AddToInventory(grabbedItem, ((InventorySlot)newSlot).Position))
				{
					if (grabbedSlot is InventorySlot)
					{
						if (!RunManager.Instance.Inventory.AddToInventory(grabbedItem, ((InventorySlot)grabbedSlot).Position))
						{
							grabbedItem.SwapDimensions();
							RunManager.Instance.Inventory.AddToInventory(grabbedItem, ((InventorySlot)grabbedSlot).Position);
						}
					}
					else if(grabbedSlot is EquipmentSlot)
					{
						((EquipmentSlot)grabbedSlot).Player.EquipEquipment(((Equipment)grabbedItem), grabbedTrinketSlot);
					}
					else
					{
						dropArea.AddToDrop(grabbedItem);
					}
				}
			}
			else
			{
				if (grabbedItem is Equipment)
				{
					if (((EquipmentSlot)newSlot).Slot == ((Equipment)grabbedItem).slot)
					{
						if(((EquipmentSlot)newSlot).Item == null)
						{
							((EquipmentSlot)newSlot).Player.EquipEquipment(((Equipment)grabbedItem), ((EquipmentSlot)newSlot).TrinketIndex);
						}
						else if(grabbedSlot is InventorySlot)
						{
							RunManager.Instance.Inventory.AddToInventory(grabbedItem, ((InventorySlot)grabbedSlot).Position);
						}
						else
						{
							((EquipmentSlot)grabbedSlot).Player.EquipEquipment(((Equipment)grabbedItem), ((EquipmentSlot)grabbedSlot).TrinketIndex);
						}
					}
					else
					{
						if (grabbedSlot is InventorySlot)
						{
							RunManager.Instance.Inventory.AddToInventory(grabbedItem, ((InventorySlot)grabbedSlot).Position);
						}
						else
						{
							((EquipmentSlot)grabbedSlot).Player.EquipEquipment(((Equipment)grabbedItem));
						}
					}
				}
				else
				{
					if (grabbedSlot is InventorySlot)
					{
						RunManager.Instance.Inventory.AddToInventory(grabbedItem, ((InventorySlot)grabbedSlot).Position);
					}
					else
					{
						((EquipmentSlot)grabbedSlot).Player.EquipEquipment(((Equipment)grabbedItem), grabbedTrinketSlot);
					}
				}
			}

			grabbedSlot.GrabItem(false);
			grabbedSlot = null;
		}
		*/

		/*
		public void PlaceItem(IDroppable droppable)
		{
			if (grabbedSlot != null)
			{
				if (droppable.IsInfiniteStorage())
				{
					Item grabbedItem = grabbedSlot.Item;
					int grabbedTrinketSlot = -1;

					if (grabbedSlot is InventorySlot)
					{
						RunManager.Instance.Inventory.RemoveFromInventory(grabbedItem);
					}
					else if (grabbedSlot is EquipmentSlot)
					{
						grabbedTrinketSlot = ((EquipmentSlot)grabbedSlot).TrinketIndex;
						((EquipmentSlot)grabbedSlot).Player.UnequipEquipment(((Equipment)grabbedItem).slot, grabbedTrinketSlot);
					}
					else
					{
						dropArea.RemoveFromDrop(grabbedItem);
					}

					dropArea.AddToDrop(grabbedItem);
					grabbedSlot.GrabItem(false);
					grabbedSlot = null;
					return;
				}

				PlaceItem(droppable.GetSlot());
			}
		}
		*/
		/*
		public void Display()
		{
			if (slots == null)
				return;

			if (slots[0, 0] == null)
			{
				Populate();
			}

			foreach (var slot in slots)
			{
				slot.SetItem(null);
			}

			Vector2Int dim = new Vector2Int(RunManager.Instance.Inventory.inventory.GetLength(0), RunManager.Instance.Inventory.inventory.GetLength(1));
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
		}*/
	}
}