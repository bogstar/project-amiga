using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentModifierSO : ScriptableObject
{
	public abstract void OnEquip(params object[] parameters);
	public abstract void OnUnequip(params object[] parameters);
}