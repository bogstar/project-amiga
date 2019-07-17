using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Disable Effect", menuName = "Status Effects/Behaviours/Disable")]
public class DisableBehaviour : StatusEffectBehaviour
{
	public bool disablesAttacks;
	public bool disablesSpells;
	public bool disablesAction;
	public bool disablesDraw;

	public override void OnAfflicted(params object[] parameters)
	{
	}

	public override void OnUnafflicted(params object[] parameters)
	{
	}

	public override void OnAttacked(params object[] parameters)
	{
	}

	public override void TickPerTurn(params object[] parameters)
	{
	}
}