using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Characters/Card")]
public class CardPlayerScriptableObject : AbilityScriptableObject
{
	[TextArea]
	public string description;
	public Sprite picture;
	public Type type;
	public Rarity rarity;
	public int manaCost;
	public string tooltip;

	public StepValue[] stepValue;

	public enum Type { Attack, Spell, Skill, Power }
	public enum Rarity { Common }

	public AbilityStepsWithTargetingData_Player[] abilityStepsWithTargetingData;

	[System.Serializable]
	public struct StepValue
	{
		public int descriptionTag;
		public int stepIndex;
		public int valueIndex;
	}
}

[System.Serializable]
public class AbilityStepsWithTargetingData_Player : AbilityStepWithTargetingData_Base
{
	public new TargetingData_Player targetingData;
}

[System.Serializable]
public class TargetingData_Player : TargetingData_Base
{
	public string textForChoosing;
}