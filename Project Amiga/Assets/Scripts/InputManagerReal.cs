using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Amiga.UI.Inventory;
using Amiga.Inventory;

public class InputManagerReal : Singleton<InputManagerReal>
{
	EventSystem eventSystem;
	public GameObject tooltipPrefab;

	GameObject tooltip;

	protected override void Awake()
	{
		base.Awake();

		eventSystem = FindObjectOfType<EventSystem>();
	}

	void LateUpdate()
	{
		if (GameManager.Instance.GameState != GameManager.State.Dungeon)
		{
			//return;
		}

		if (tooltip == null)
		{
			tooltip = Instantiate(tooltipPrefab, FindObjectOfType<Canvas>().transform);
			tooltip.SetActive(false);
		}

		RectTransform t = tooltip.transform as RectTransform;
		t.anchorMax = Vector2.zero;
		t.anchorMin = Vector2.zero;

		PointerEventData pointer = new PointerEventData(eventSystem);
		float scaleFactor = 1;
		Canvas[] canvi = FindObjectsOfType<Canvas>();
		foreach (var canvas in canvi)
		{
			if (canvas.name == "GUI")
			{
				scaleFactor = canvas.scaleFactor;
			}
		}
		pointer.position = new Vector2(Input.mousePosition.x / scaleFactor, Input.mousePosition.y / scaleFactor);
		float cursorThickness = 8 / scaleFactor;

		//Vector2 screen = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		Vector2 screen = new Vector2(Screen.width / scaleFactor, Screen.height / scaleFactor);
		int signx = 1;
		int signy = 1;

		if ((pointer.position.x + cursorThickness + t.sizeDelta.x * t.localScale.x) > screen.x)
		{
			signx = -1;
		}

		if (pointer.position.y < t.sizeDelta.y * t.localScale.y)
		{
			signy = -1;
		}

		t.anchoredPosition = new Vector3(pointer.position.x + t.sizeDelta.x * t.localScale.x / 2 * signx, pointer.position.y - t.sizeDelta.y * t.localScale.y / 2 * signy) + new Vector3((signx == -1) ? 0 : cursorThickness, 0, 0);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Amiga.UI.SceneSettings sceneSettings = GameManager.Instance.CurrentSceneSettings;
			sceneSettings.HideTopPanel();
		}

		if (GameManager.Instance.GameState != GameManager.State.Dungeon)
		{
			//return;
		}

		if (eventSystem == null)
		{
			eventSystem = FindObjectOfType<EventSystem>();
		}

		PointerEventData pointer = new PointerEventData(eventSystem);
		pointer.position = Input.mousePosition;
		List<RaycastResult> raycastResults = new List<RaycastResult>();

		eventSystem.RaycastAll(pointer, raycastResults);

		if (GameManager.Instance.CurrentSceneSettings == null)
		{
			return;
		}

		if (!(GameManager.Instance.CurrentSceneSettings is Amiga.UI.Dungeon.DungeonDisplay))
		{
			//return;
		}

		//Amiga.UI.Dungeon.InventoryMenu inventoryMenu = ((Amiga.UI.Dungeon.DungeonDisplay)GameManager.Instance.CurrentSceneSettings).inventoryPanel;

		if (Input.GetMouseButton(0))
		{
			if (Input.GetMouseButtonDown(1))
			{
				RotateGrabbedItem();
			}
		}

		foreach (var result in raycastResults)
		{
			if (result.gameObject.GetComponent<ITooltip>() != null)
			{
				ITooltip tooltippable = result.gameObject.GetComponent<ITooltip>();
				UISlot.TooltipData tooltipData = tooltippable.GetTooltip();

				tooltip.gameObject.SetActive(tooltipData.show);

				if (tooltipData.show)
				{
					tooltip.transform.Find("Description").GetComponent<Text>().text = "<b>" + tooltipData.title + "</b>\n";
					tooltip.transform.Find("Description").GetComponent<Text>().text += tooltipData.content;
					tooltip.transform.Find("Image Holder").Find("Image").GetComponent<Image>().sprite = tooltipData.image;
				}

				break;
			}
			else
			{
				tooltip.gameObject.SetActive(false);
			}
		}

		bool itstrue = false;
		foreach (var result in raycastResults)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (result.gameObject.GetComponent<UISlot>() != null)
				{
					UISlot grabbable = result.gameObject.GetComponent<UISlot>();

					GrabItem(grabbable);
				}
			}

			if (Input.GetMouseButtonUp(0))
			{
				if (result.gameObject.GetComponent<DroppableArea>() != null)
				{
					DroppableArea placeable = result.gameObject.GetComponent<DroppableArea>();

					PlaceItem(placeable);
				}
			}

			/*
			if (result.gameObject.GetComponent<UISlot>() != null)
			{
				UISlot slot = result.gameObject.GetComponent<UISlot>();

				if (slot is EquipmentSlot)
				{
					//Amiga.UI.Dungeon.EquipmentSlot dSlot = slot as Amiga.UI.Dungeon.EquipmentSlot;

					//dSlot.Highlight(true);

					/*
					if (Input.GetMouseButtonDown(0))
					{
						inventoryMenu.GrabItem(dSlot);
					}

					if (Input.GetMouseButtonUp(0))
					{
						inventoryMenu.PlaceItem(dSlot);
					}
					*//*
				}
				else if(slot is Amiga.UI.Inventory.InventorySlot)
				{
					Amiga.UI.Inventory.InventorySlot iSlot = slot as Amiga.UI.Inventory.InventorySlot;

					inventoryMenu.HighlightInventory(iSlot);

					itstrue = true;

					/*
					if (Input.GetMouseButtonDown(0))
					{
						inventoryMenu.GrabItem(iSlot);
					}

					if (Input.GetMouseButtonUp(0))
					{
						inventoryMenu.PlaceItem(iSlot);
					}
					*//*

					break;
				}
			}*/
		}

		if (Input.GetMouseButtonUp(0))
		{
			ReleaseItem();
		}

		/*
		if (itstrue == false)
		{
			if(GameManager.Instance.CurrentSceneSettings != null && GameManager.Instance.CurrentSceneSettings is Amiga.UI.Dungeon.DungeonDisplay)
			{
				inventoryMenu.UnhighlightAll();
			}
		}
		*/
	}

	UISlot grabbedSlot;

	void GrabItem(UISlot grabbable)
	{
		if (grabbable.Item == null)
		{
			return;
		}

		if (grabbable is InventorySlot inventoryGrabbable)
		{
			grabbedSlot = inventoryGrabbable.MainSlot;
			inventoryGrabbable.MainSlot.GrabItem(true);
		}
		else
		{
			grabbedSlot = grabbable;
			grabbable.GrabItem(true);
		}
	}

	void ReleaseItem()
	{
		if (grabbedSlot != null)
		{
			PlaceItem(grabbedSlot.GetComponent<DroppableArea>());
		}
	}

	void RotateGrabbedItem()
	{
		if (grabbedSlot != null)
		{
			grabbedSlot.Item.SwapDimensions();
		}
	}

	void PlaceItem(DroppableArea droppableArea)
	{
		if (grabbedSlot == null)
		{
			return;
		}

		Item grabbedItem = grabbedSlot.Item;
		int grabbedTrinketSlot = -1;

		/*
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
		*/


		IDroppable droppable = droppableArea.droppable;
		if (droppable.DropHere(grabbedItem, droppableArea.slot))
		{
			grabbedSlot.GetComponent<RemovableArea>().removable.RemoveFromHere(grabbedSlot);
		}

		grabbedSlot.GrabItem(false);
		grabbedSlot = null;

		/*
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
				else if (grabbedSlot is EquipmentSlot)
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
					if (((EquipmentSlot)newSlot).Item == null)
					{
						((EquipmentSlot)newSlot).Player.EquipEquipment(((Equipment)grabbedItem), ((EquipmentSlot)newSlot).TrinketIndex);
					}
					else if (grabbedSlot is InventorySlot)
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
		grabbedSlot = null;*/
	}
}