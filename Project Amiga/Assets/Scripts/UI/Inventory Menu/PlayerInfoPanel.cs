using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amiga.Inventory;

namespace Amiga.UI.Inventory
{
	public class PlayerInfoPanel : MonoBehaviour
	{
		[Header("Prefabs")]
		[SerializeField]
		GameObject equipmentSlotPrefab;

		[Header("References")]
		[SerializeField]
		RectTransform slotsHolder;
		public Text title;
		public Text stats;
		public Image healthOverlay;
		public Text healthText;
		public Text manaText;
		public Text numberOfCardsText;

		float healthOverlayStartWidth = 74.44f;

		public Player Player { get; private set; }
		List<EquipmentSlot> slots = new List<EquipmentSlot>();


		public void Init (Player player, EquipmentPanel equipmentPanel)
		{
			this.Player = player;

			Equipment.Slot[] allSlots = (Equipment.Slot[])System.Enum.GetValues(typeof(Equipment.Slot));

			for (int i = 0; i < allSlots.Length; i++)
			{
				if (allSlots[i] == Equipment.Slot.Trinket)
				{
					for (int j = 0; j < GameManager.Instance.equipmentManager.trinketCount; j++)
					{
						InstantiateSlot(player, Equipment.Slot.Trinket, ((string[])System.Enum.GetNames(typeof(Equipment.Slot)))[i], i + j, equipmentPanel, j);
					}
				}
				else
				{
					InstantiateSlot(player, allSlots[i], ((string[])System.Enum.GetNames(typeof(Equipment.Slot)))[i], i, equipmentPanel);
				}
			}

			Refresh();
		}

		void InstantiateSlot(Player player, Equipment.Slot slot, string name, int index, EquipmentPanel equipmentPanel, int trinketIndex = -1)
		{
			GameObject slotGO = (GameObject)Instantiate(equipmentSlotPrefab, slotsHolder);

			RectTransform slotTransform = ((RectTransform)slotGO.transform);
			slotTransform.anchorMax = new Vector2(0, 1);
			slotTransform.anchorMin = new Vector2(0, 1);
			int row = index / 2;
			index %= 2;
			slotTransform.anchoredPosition = new Vector2(index * slotTransform.sizeDelta.x, - row * slotTransform.sizeDelta.y) + new Vector2(slotTransform.sizeDelta.x / 2, - slotTransform.sizeDelta.y / 2);

			slotGO.name = name + (trinketIndex > -1 ? (" " + trinketIndex.ToString()) : "");
			EquipmentSlot newSlot = slotGO.GetComponentInChildren<EquipmentSlot>();
			slots.Add(newSlot);
			slots[slots.Count - 1].Init(player, slot, trinketIndex);
			slotGO.transform.Find("Equipment Piece").GetComponent<Text>().text = name + ":";

			newSlot.gameObject.GetComponentInChildren<DroppableArea>().droppable = equipmentPanel;
			newSlot.gameObject.GetComponentInChildren<RemovableArea>().removable = equipmentPanel;
		}

		[System.Serializable]
		public struct SlotEquipmentPair
		{
			public EquipmentSlot slot;
			public Equipment.Slot slotType;
		}

		public void Refresh()
		{
			title.text = Player.Name;
			stats.text =
				"Critical Chance: " + (Player.stats.criticalChance.Value * 100).ToString() + "%" +
				"\nArmor: " + (Player.stats.armor.Value * 100).ToString() + "%" +
				"\nMagic Resist: " + (Player.stats.magicResist.Value * 100).ToString() + "%" +
				"\nArmor Penetration: " + (Player.stats.armorPenetration.Value * 100).ToString() + "%" +
				"\nMagic Penetration: " + (Player.stats.magicPenetration.Value * 100).ToString() + "%" +
				"\nDamage Multiplier: " + (Player.stats.rawDamageMultiplier.Value * 100).ToString() + "%";

			healthOverlay.rectTransform.sizeDelta = new Vector2(Player.Health / (float)Player.MaxHealth * healthOverlayStartWidth, healthOverlay.rectTransform.sizeDelta.y);
			healthText.text = Player.Health + "/" + Player.MaxHealth;
			manaText.text = Player.mana.ToString();

			int trinketIndex = 0;

			foreach (var slot in slots)
			{
				Equipment.Slot targetSlot = slot.Slot;

				if(targetSlot == Equipment.Slot.Trinket)
				{
					slot.SetItem(Player.currentEquipment[targetSlot][trinketIndex]);
					trinketIndex++;
				}
				else
				{
					slot.SetItem(Player.currentEquipment[targetSlot][0]);
				}
			}

			numberOfCardsText.text = "Cards\n(" + Player.cards.Count + ")";
		}
	}
}