using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityStep_Base : ScriptableObject
{
	public abstract OnPlayResult OnPlay(params object[] parameters);
	public virtual string GetValueForIndex(int index) { return null; }
	
	public struct OnPlayResult
	{
		public int unshieldedDamage;
		public HealthEntityCombatObject summonedEntity;
		public StatusEffectScriptableObject effectApplied;
	}
}