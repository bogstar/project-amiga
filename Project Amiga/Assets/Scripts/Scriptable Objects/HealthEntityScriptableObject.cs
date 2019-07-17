using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthEntityScriptableObject : ScriptableObject
{
	public new string name;
	public GameObject model;
	public int health;
	[Range(0, 1)]
	public float armor;
	[Range(0, 1)]
	public float magicResist;
	[Range(0, 1)]
	public float armorPenetration;
	[Range(0, 1)]
	public float magicPenetration;
	[Range(0, 1)]
	public float criticalChance;
	public float rawDamageMultiplier;
}