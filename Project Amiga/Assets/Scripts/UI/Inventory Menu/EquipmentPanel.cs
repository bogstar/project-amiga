using System.Collections;
using System.Collections.Generic;
using Amiga.Inventory;
using UnityEngine;

namespace Amiga.UI.Inventory
{
	public class EquipmentPanel : MonoBehaviour, IDroppable, IRemovable
	{
		[Header("References")]
		[SerializeField]
		PlayerInfoPanel player1Panel;
		[SerializeField]
		PlayerInfoPanel player2Panel;

		void Start()
		{
			player1Panel.Init(RunManager.Instance.player1, this);
			player2Panel.Init(RunManager.Instance.player2, this);
		}

		bool EquipEquipment(Equipment equipment, EquipmentSlot slot)
		{
			Player player = slot.Player;

			if (slot.Slot == Equipment.Slot.Trinket)
			{
				if (player.EquipEquipment(equipment, slot.TrinketIndex))
				{
					return true;
				}
			}
			else
			{
				if (player.EquipEquipment(equipment))
				{
					return true;
				}
			}

			return false;
		}

		void UnequipEquipment(EquipmentSlot slot)
		{
			Player player = slot.Player;

			player.UnequipEquipment(slot.Slot, slot.TrinketIndex);
		}

		public bool DropHere(Item item, UISlot targetSlot)
		{
			if (EquipEquipment(item as Equipment, targetSlot as EquipmentSlot))
			{
				player1Panel.Refresh();
				player2Panel.Refresh();
				return true;
			}

			return false;
		}

		public void RemoveFromHere(UISlot slot)
		{
			UnequipEquipment(slot as EquipmentSlot);
			player1Panel.Refresh();
			player2Panel.Refresh();
		}
	}
}