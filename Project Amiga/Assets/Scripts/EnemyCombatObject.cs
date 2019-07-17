using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombatObject : HealthEntityCombatObject
{
	public bool clickableTileClickable;
	public EnemyUI ui;

	public Enemy.AbilitiesWithChance[] Abilities { get; private set; }
	public EnemyAbilityScriptableObject NextAbility { get; private set; }
	public List<List<HealthEntityCombatObject>> targetsPerAbilityStep { get; private set; }

	public AudioClip[] attackSounds;
	public AudioClip[] deathSounds;

	public int enemyIndex;

	bool doingTurn;
	bool doneTurn;

	/// <summary>
	/// Spawn a model and initialize the combat entity.
	/// </summary>
	/// <param name="enemy">Entity</param>
	/// <param name="index">Index in the array of enemies.</param>
	public void Init(Enemy enemy, int index, System.Action<HealthEntityCombatObject> OnDeathCallback)
	{
		// Pass the info to Health Entity.
		if (Init(enemy as HealthEntity, OnDeathCallback) == null)
		{
			return;
		}

		Abilities = enemy.abilities;
		attackSounds = enemy.attackSounds;
		deathSounds = enemy.deathSounds;
		enemyIndex = index;

		RefreshUI();
	}

	public void SetTargetsPerAbilityStep(List<List<HealthEntityCombatObject>> targets)
	{
		targetsPerAbilityStep = targets;
	}
	
	public void DoTurn(ref bool doing)
	{
		if (doingTurn == false && doneTurn == false)
		{
			doingTurn = doing = true;
			//EnemyAbilityScriptableObject nextAbilityTargetable = (EnemyAbilityScriptableObject)nextAbility;

			StartCoroutine(DoAbility());
		}
		else if(doingTurn == false && doneTurn == true)
		{
			doing = false;
			doneTurn = false;
		}
	}
	
	public void ChooseNextAbility()
	{
		int totalChance = 0;
		foreach (var a in Abilities)
		{
			totalChance += a.chance;
		}

		int randomIndex = Random.Range(0, totalChance);

		int anotherChance = -1;
		foreach (var a in Abilities)
		{
			anotherChance += a.chance;
			if (anotherChance >= randomIndex)
			{
				NextAbility = a.ability;
				break;
			}
		}

		RefreshUI();
	}

	IEnumerator DoAbility()
	{
		for (int i = 0; i < targetsPerAbilityStep.Count; i++)
		{
			Animator.SetTrigger(NextAbility.animatorTrigger);

			NextAbility.abilityStepsWithTargetingData[i].abilityStep.OnPlay(targetsPerAbilityStep[i], this);
		}

		FindObjectOfType<AudioManager>().PlayRandomClip(NextAbility.soundsForPlay);
		FindObjectOfType<AudioManager>().PlayRandomClip(attackSounds);
		
		yield return new WaitForSeconds(1.5f);
		NextAbility = null;
		RefreshUI();
		doingTurn = false;
		doneTurn = true;
		yield return null;
	}

	protected override void Die()
	{
		base.Die();

		NextAbility = null;
		foreach (var dropable in ModelInfo.dropables)
		{
			dropable.transform.SetParent(transform.parent);
			dropable.GetComponent<Collider>().enabled = true;
			Rigidbody rb = dropable.AddComponent<Rigidbody>();
			rb.drag = .2f;
			rb.AddForce(new Vector3(Random.Range(-1, 1), 1, Random.Range(-1, 1)).normalized + Vector3.up * 2, ForceMode.Impulse);
		}

		RefreshUI();
		StartCoroutine(DestroyMe());
	}

	IEnumerator DestroyMe()
	{
		yield return new WaitForSeconds(5f);
		foreach (var drop in ModelInfo.dropables)
		{
			GameObject.Destroy(drop.gameObject);
		}
		GameObject.Destroy(gameObject);
	}

	public override void OnTurnStart()
	{
		base.OnTurnStart();

		RefreshUI();
	}

	public override void RefreshUI()
	{
		ui.UpdateUI();
	}
}