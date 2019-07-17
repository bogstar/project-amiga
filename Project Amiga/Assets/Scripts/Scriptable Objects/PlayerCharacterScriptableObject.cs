using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amiga.Inventory;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/Character")]
public class PlayerCharacterScriptableObject : HealthEntityScriptableObject
{
	public int mana;

	[Header("Starting Equipment")]
	public EquipmentCollection startingEquipment;
	public List<NativePowerScriptableObject> nativePowers;

	[System.Serializable]
	public struct EquipmentCollection : IEnumerable
	{
		public EquipmentScriptableObject head;
		public EquipmentScriptableObject armor;
		public EquipmentScriptableObject feet;
		public EquipmentScriptableObject mainHand;
		public EquipmentScriptableObject offHand;
		public EquipmentScriptableObject[] trinkets;

		private IEnumerable<EquipmentScriptableObject> Events()
		{
			yield return head;
			yield return armor;
			yield return feet;
			yield return mainHand;
			yield return offHand;
			foreach (var trinket in trinkets)
			{
				yield return trinket;
			}
		}

		public IEnumerator<EquipmentScriptableObject> GetEnumerator()
		{
			return Events().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}