using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStepWithTargetingData_Base
{
	public AbilityStep_Base abilityStep;
	public TargetingData_Base targetingData;
}

public class TargetingData_Base
{
	public TargetingType targetingType;

	public int useTargetsFromStepNumber;
	public bool onlySelf;

	public TargetAlignment targetAlignment;

	public Target targets;
	public int numberOfTargets;

	public bool selfIncluded;

	public enum TargetingType { Normal, PreviousStep, NoTargeting }
	public enum Target { None, One, Multiple, MultipleSpecific, All }
	public enum TargetAlignment { None, Friendly, Hostile, Both }

	public bool canTargetDead;
}

[System.Serializable]
public class TargetingDataa
{
	public bool onlySelf;
	public TargetAlignment targetAlignment;
	public Target targets;
	public int numberOfTargets;
	public bool selfIncluded;
	public bool canTargetDead;
	public string textForTargeting;

	public enum Target { None, One, Multiple, MultipleSpecific, All }
	public enum TargetAlignment { None, Friendly, Hostile, Both }
}