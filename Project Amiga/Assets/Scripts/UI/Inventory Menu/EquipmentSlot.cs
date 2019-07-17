using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amiga.Inventory;

namespace Amiga.UI.Inventory
{
	public class EquipmentSlot : UISlot
	{
		public Player Player { get; private set; }
		public Equipment.Slot Slot { get; private set; }
		public int TrinketIndex { get; private set; }


		public bool IsInfiniteStorage()
		{
			return false;
		}

		public void Init(Player player, Equipment.Slot slot, int trinketIndex = -1)
		{
			Player = player;
			Slot = slot;
			TrinketIndex = trinketIndex;
		}

		public override void SetItem(Item item)
		{
			if (item == null)
			{
				ItemImage.gameObject.SetActive(false);
			}
			else if (item is Equipment equipment)
			{
				Item = equipment;

				ItemImage.gameObject.SetActive(true);
				ItemImage.sprite = item.Image;
			}
		}
	}
}