using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffectBehaviour : ScriptableObject
{
	public abstract void TickPerTurn(params object[] parameters);
	public abstract void OnAttacked(params object[] parameters);
	public abstract void OnAfflicted(params object[] parameters);
	public abstract void OnUnafflicted(params object[] parameters);
}