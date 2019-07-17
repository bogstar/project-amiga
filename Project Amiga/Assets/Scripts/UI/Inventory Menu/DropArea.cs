using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amiga.Inventory;

namespace Amiga.UI.Inventory
{
	public class DropArea : MonoBehaviour, IDroppable, IRemovable
	{
		[Header("Prefabs")]
		[SerializeField]
		GameObject equipmentSlotPrefab;

		[Header("References")]
		[SerializeField]
		ScrollRect dropArea;

		List<Item> discardedItems = new List<Item>();


		void Start()
		{
			dropArea.viewport.GetComponent<DroppableArea>().droppable = this;
			Refresh();
		}

		public void DiscardItems()
		{
			discardedItems.Clear();
			Refresh();
		}

		public void AddToDrop(Item item)
		{
			discardedItems.Add(item);
			Refresh();
		}

		public void RemoveFromDrop(Item item)
		{
			discardedItems.Remove(item);
			Refresh();
		}

		public void Refresh()
		{
			foreach (Transform t in dropArea.content)
			{
				Destroy(t.gameObject);
			}

			foreach (var item in discardedItems)
			{
				DropAreaSlot newSlot = Instantiate(equipmentSlotPrefab, dropArea.content).GetComponent<DropAreaSlot>();
				newSlot.gameObject.GetComponentInChildren<RemovableArea>().removable = this;
				newSlot.SetItem(item);
			}
		}

		public void OnDrop(Item item)
		{
			AddToDrop(item);
		}

		public bool IsInfiniteStorage()
		{
			return true;
		}

		public UISlot GetSlot()
		{
			return null;
		}

		public bool DropHere(Item item, UISlot targetSlot)
		{
			AddToDrop(item);
			Refresh();
			return true;
		}

		public void RemoveFromHere(UISlot slot)
		{
			RemoveFromDrop(slot.Item);
			Refresh();
		}
	}
}