using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amiga.Inventory
{
	[System.Serializable]
	public class Equipment : Item
	{
		public PlayerCharacterScriptableObject[] Wearers { get; private set; }

		public Dictionary<EquipmentScriptableObject.GemColor, CardPlayerScriptableObject[]> GemCardPairs { get; private set; }
		public EquipmentScriptableObject.Socket[] Socket { get; private set; }

		public Slot slot { get; private set; }
		public EquipmentModifierSO[] Modifiers { get; private set; }
		public EquipmentScriptableObject.GemColor[] CurrentSockets;
		public int SocketCount { get { return CurrentSockets.Length; } }

		public Equipment(EquipmentScriptableObject data) : base(data)
		{
			GemCardPairs = new Dictionary<EquipmentScriptableObject.GemColor, CardPlayerScriptableObject[]>();
			if (data.wearers.Length < 1)
			{
				Debug.LogWarning("Wearers for equipment: " + data + " are empty.");
			}
			Wearers = data.wearers;

			Socket = data.socket;

			/*bool validGCP = false;

			foreach (var socket in data.socket)
			{
				foreach (var gemCardPair in socket.gemCardPair)
				{
					gemCardPair.
				}
			}


			foreach (var gcp in data.gemCardPairs)
			{
				if (gcp.gemColor == EquipmentScriptableObject.GemColor.Empty)
				{
					validGCP = true;
					break;
				}
			}
			if (validGCP)
			{
				foreach (var gcp in data.gemCardPairs)
				{
					if (!GemCardPairs.ContainsKey(gcp.gemColor))
					{
						GemCardPairs.Add(gcp.gemColor, gcp.cardCollection);
					}
					else
					{
						Debug.LogError("In Gem-Card Pairs for equipment: " + data + ", there is a duplicate color (" + gcp.gemColor.ToString() + ").");
					}
				}
			}
			else if(data.socketNumber > 0)
			{
				Debug.LogWarning("In Gem-Card Pairs for equipment: " + data + ", an empty gem option doesn't exist.");
			}*/
			slot = data.slot;
			Modifiers = data.modifiers;
			
			CurrentSockets = new EquipmentScriptableObject.GemColor[Socket.Length];

			for (int i = 0; i < CurrentSockets.Length; i++)
			{
				CurrentSockets[i] = EquipmentScriptableObject.GemColor.Empty;
			}
		}

		/// <summary>
		/// Returns true if successful and false if not.
		/// </summary>
		/// <param name="gem"></param>
		public bool InsertGem(EquipmentScriptableObject.GemColor gem)
		{
			for (int i = 0; i < CurrentSockets.Length; i++)
			{
				if (CurrentSockets[i] == EquipmentScriptableObject.GemColor.Empty)
				{
					CurrentSockets[i] = gem;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Returns true if succesful and false if not.
		/// </summary>
		/// <param name="gem"></param>
		/// <returns></returns>
		public bool RemoveGem(EquipmentScriptableObject.GemColor gem)
		{
			for (int i = 0; i < CurrentSockets.Length; i++)
			{
				if (CurrentSockets[i] == gem)
				{
					CurrentSockets[i] = EquipmentScriptableObject.GemColor.Empty;
					return true;
				}
			}
			return false;
		}

		public enum Slot { Head, Armor, Feet, MainHand, OffHand, Trinket }
	}
}