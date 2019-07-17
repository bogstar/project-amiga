using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
	public CardPlayerScriptableObject data { get; private set; }
	public string name;
	public Sprite image;
	public string description;
	public int manaCost;
	public AbilityStep_Base.OnPlayResult[] onPlayResults;
	public AbilityStepsWithTargetingData_Player[] abilityStepsWithTargetingData;
	public AudioClip[] soundForPlay;
	public CardPlayerScriptableObject.StepValue[] stepValue;
	public string tooltip;

	public Card(CardPlayerScriptableObject unsafeCopy)
	{
		data = ScriptableObject.Instantiate<CardPlayerScriptableObject>(unsafeCopy);

		this.name = unsafeCopy.name;
		this.image = unsafeCopy.picture;
		this.description = unsafeCopy.description;
		this.manaCost = unsafeCopy.manaCost;
		this.soundForPlay = unsafeCopy.soundsForPlay;
		this.abilityStepsWithTargetingData = unsafeCopy.abilityStepsWithTargetingData;
		this.onPlayResults = new AbilityStep_Base.OnPlayResult[abilityStepsWithTargetingData.Length];
		this.stepValue = unsafeCopy.stepValue;
		this.tooltip = unsafeCopy.tooltip;
	}
}