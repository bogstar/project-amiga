using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Native Power", menuName = "Characters/Native Powers/Native Power")]
public class NativePowerScriptableObject : ScriptableObject
{
	public new string name;
	public Sprite picture;
	[TextArea()]
	public string description;
	public int manaCost;
	public int cooldown;
	public CooldownType cooldownType;
	public AudioClip[] soundsForPlay;
	public AbilityStep_Base.OnPlayResult[] onPlayResults;

	public List<int> values;

	public AbilityStepsWithTargetingData_Player[] abilityStepsWithTargetingData;

	public enum CooldownType { Dungeon, Combat, Turn }
}