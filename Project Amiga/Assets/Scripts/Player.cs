using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amiga.Inventory;

[System.Serializable]
public class Player : HealthEntity
{
	public int mana;

	public Dictionary<Equipment.Slot, Equipment[]> currentEquipment;

	public List<Card> cards = new List<Card>();

	public List<NativePowerScriptableObject> nativePowers = new List<NativePowerScriptableObject>();


	public Player(PlayerCharacterScriptableObject data) : base(data)
	{
		alignment = Alignment.Good;

		mana = data.mana;

		int equipmentSlotsCount = System.Enum.GetNames(typeof(Equipment.Slot)).Length;
		currentEquipment = new Dictionary<Equipment.Slot, Equipment[]>();

		for (int i = 0; i < equipmentSlotsCount; i++)
		{
			int slotsCount = 1;

			if((Equipment.Slot)System.Enum.GetValues(typeof(Equipment.Slot)).GetValue(i) == Equipment.Slot.Trinket)
			{
				slotsCount = 5;
			}

			currentEquipment.Add((Equipment.Slot)System.Enum.GetValues(typeof(Equipment.Slot)).GetValue(i), new Equipment[slotsCount]);
		}

		foreach (var e in data.startingEquipment)
		{
			if (e != null)
			{
				EquipEquipment(Item.CreateItem(e) as Equipment);
			}
		}

		nativePowers = data.nativePowers;
	}

	public bool InsertGem(EquipmentScriptableObject.GemColor gem, Equipment.Slot slot, int trinketSlot = -1)
	{
		if (slot == Equipment.Slot.Trinket)
		{
			if (trinketSlot < 0)
			{
				return false;
			}

			if (currentEquipment[slot][trinketSlot].SocketCount < 1)
			{
				return false;
			}

			if (currentEquipment[Equipment.Slot.Trinket][trinketSlot].InsertGem(gem))
			{
				RefreshCards();
				return true;
			}

			return false;
		}
		else
		{
			if (currentEquipment[slot][0].SocketCount < 1)
			{
				return false;
			}

			if (currentEquipment[slot][0].InsertGem(gem))
			{
				RefreshCards();
				return true;
			}

			return false;
		}
	}

	public bool RemoveGem(EquipmentScriptableObject.GemColor gem, Equipment.Slot slot, int trinketSlot = -1)
	{
		if (slot == Equipment.Slot.Trinket)
		{
			if (trinketSlot < 0)
			{
				return false;
			}

			if (currentEquipment[slot][trinketSlot].SocketCount < 1)
			{
				return false;
			}

			if (currentEquipment[Equipment.Slot.Trinket][trinketSlot].RemoveGem(gem))
			{
				RefreshCards();
				return true;
			}

			return false;
		}
		else
		{
			if (currentEquipment[slot][0].SocketCount < 1)
			{
				return false;
			}

			if (currentEquipment[slot][0].RemoveGem(gem))
			{
				RefreshCards();
				return true;
			}

			return false;
		}
	}

	public Equipment UnequipEquipment(Equipment.Slot slot, int trinketSlot = -1)
	{
		Equipment unequippedItem = null;

		if (slot == Equipment.Slot.Trinket)
		{
			if (trinketSlot < 0)
			{
				return null;
			}
			else
			{
				unequippedItem = currentEquipment[Equipment.Slot.Trinket][trinketSlot];
				currentEquipment[Equipment.Slot.Trinket][trinketSlot] = null;
			}
		}
		else
		{
			unequippedItem = currentEquipment[slot][0];
			currentEquipment[slot][0] = null;
		}

		if(unequippedItem != null)
		{
			foreach (var modifier in unequippedItem.Modifiers)
			{
				modifier.OnUnequip(this);
			}
		}

		RefreshCards();

		return unequippedItem;
	}

	public void RefreshCards()
	{
		cards.Clear();

		foreach (var kvp in currentEquipment)
		{
			if (kvp.Key == Equipment.Slot.Trinket)
			{
				foreach (var item in kvp.Value)
				{
					AddCards(item);
				}
			}
			else
			{
				AddCards(kvp.Value[0]);
			}
		}
	}

	void AddCards(Equipment item)
	{
		if (item == null)
		{
			return;
		}
		
		var Sockets = item.Socket;

		for (int i = 0; i < Sockets.Length; i++)
		{
			var socket = Sockets[i];

			for (int j = 0; j < socket.gemCardPair.Length; j++)
			{
				var gemCardPair = socket.gemCardPair[j];

				if (gemCardPair.gemColor == item.CurrentSockets[i])
				{
					foreach (var card in gemCardPair.cardCollection)
					{
						Card newCard = new Card(card);
						this.cards.Add(newCard);
					}

					break;
				}
			}
		}
		foreach (var socket in Sockets)
		{
			

		}
		
		/*
		for (int i = 0; i < item.SocketCount; i++)
		{
			

			EquipmentScriptableObject.GemColor socket = item.CurrentSockets[i];

			CardPlayerScriptableObject[] cards = item.GemCardPairs[socket];

			foreach (var card in cards)
			{
				Card newCard = new Card(card);
				this.cards.Add(newCard);
			}
		}*/
	}

	public bool EquipEquipment(Equipment newEquipment, int trinketSlot = -1)
	{
		bool found = false;
		foreach (var wearer in newEquipment.Wearers)
		{
			if (wearer.name == Name)
			{
				found = true;
				break;
			}
		}
		if (!found)
		{
			return false;
		}

		if (newEquipment.slot == Equipment.Slot.Trinket)
		{
			if (trinketSlot < 0)
			{
				int firstFreeSlot = GetFirstFreeTrinketSlot();
				if (firstFreeSlot < 0)
				{
					return false;
				}
				else
				{
					currentEquipment[Equipment.Slot.Trinket][firstFreeSlot] = newEquipment;
				}
			}
			else
			{
				currentEquipment[Equipment.Slot.Trinket][trinketSlot] = newEquipment;
			}
		}
		else
		{
			currentEquipment[newEquipment.slot][0] = newEquipment;
		}

		foreach (var modifier in newEquipment.Modifiers)
		{
			modifier.OnEquip(this);
		}

		RefreshCards();
		return true;
	}

	int GetFirstFreeTrinketSlot()
	{
		int i = 0;

		foreach (var slot in currentEquipment[Equipment.Slot.Trinket])
		{
			if(slot == null)
			{
				return i;
			}
			i++;
		}

		return -1;
	}
}