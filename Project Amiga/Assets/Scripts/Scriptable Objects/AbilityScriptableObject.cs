using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityScriptableObject : ScriptableObject
{
	public new string name;

	public AudioClip[] soundsForPlay;

	public TargetingDataa[] targData;
	public AbilityStepss[] abilitySteps;
}

[System.Serializable]
public class AbilityStepss
{
	public AbilityStep_Base step;
	[Tooltip("Step from which to take targets.")]
	public int targeting;
}