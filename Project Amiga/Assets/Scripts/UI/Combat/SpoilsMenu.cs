using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amiga.Inventory;

namespace Amiga.UI.Combat
{
	public class SpoilsMenu : HUDPanel
	{
		[Header("References")]
		[SerializeField]
		Inventory.DropArea spoilsArea;

		public void Display(EncounterScriptableObject encounter)
		{
			spoilsArea.DiscardItems();
			foreach (var spoil in encounter.drops)
			{
				Item newItem = Item.CreateItem(spoil);
				spoilsArea.AddToDrop(newItem);
			}
		}
	}
}