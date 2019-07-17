using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Status Effects/Status Effect")]
public class StatusEffectScriptableObject : ScriptableObject
{
	public new string name;
	public Sprite icon;
	public Type type;

	public StatusEffectBehaviour[] behaviour;

	public enum Type
	{
		None,
		Poison,
		Thorns,
		Bubble,
		StrengthModifier,
		Restore,
		Disable
	}
}