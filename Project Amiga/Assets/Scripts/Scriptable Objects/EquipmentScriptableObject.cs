using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amiga.Inventory
{
	[CreateAssetMenu(fileName = "New Equipment", menuName = "Characters/Equipment/Equipment")]
	public class EquipmentScriptableObject : ItemScriptableObject
	{
		public PlayerCharacterScriptableObject[] wearers;
		public Equipment.Slot slot;
		public EquipmentModifierSO[] modifiers;

		public Socket[] socket;

		[System.Serializable]
		public struct GemCardPair
		{
			public GemColor gemColor;
			public CardPlayerScriptableObject[] cardCollection;
		}

		[System.Serializable]
		public struct Socket
		{
			public GemCardPair[] gemCardPair;
		}

		public enum GemColor { Empty, White, Black }
	}
}