using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amiga.Inventory;

[CreateAssetMenu(fileName = "New Gem", menuName = "Characters/Equipment/Gem")]
public class GemScriptableObject : ItemScriptableObject
{
	public EquipmentScriptableObject.GemColor color;
}