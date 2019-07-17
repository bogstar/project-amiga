using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusEffect
{
	public StatusEffectScriptableObject data;
	public int durationRemaining;
	public TickTime tickTime;

	public StatusEffect(StatusEffectScriptableObject data, TickTime tickTime, int duration)
	{
		this.data = ScriptableObject.Instantiate<StatusEffectScriptableObject>(data);

		this.tickTime = tickTime;
		durationRemaining = duration;
	}

	public enum TickTime { TurnStart, TurnEnd }
}