using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthEntityCombatObject : MonoBehaviour
{
	public HealthEntity Entity { get; protected set; }
	public ModelSetup ModelInfo { get; protected set; }
	public Animator Animator { get; protected set; }
	public ClickableArea ClickableTile { get; protected set; }

	public int Health { get { return Entity.Health; } }
	public int MaxHealth { get { return Entity.MaxHealth; } }
	public int Block { get; protected set; }
	public HealthEntity.Stats Stats { get; protected set; }
	public List<StatusEffect> StatusEffects { get; protected set; }
	public HealthEntity.Alignment alignment { get { return Entity.alignment; } }
	public bool IsDead { get; protected set; }

	System.Action<HealthEntityCombatObject> OnDeath;

	public abstract void RefreshUI();

	public delegate void ObjectParametersDelegate(params object[] parameters);

	public ObjectParametersDelegate OnAttackedBehaviour;
	public ObjectParametersDelegate OnStartTurn;
	public ObjectParametersDelegate OnEndTurn;


	/// <summary>
	/// Spawn a model and initialize the combat entity.
	/// </summary>
	/// <param name="entity"></param>
	protected HealthEntityCombatObject Init(HealthEntity entity, System.Action<HealthEntityCombatObject> OnDeathCallback)
	{
		if (entity == null)
		{
			Debug.LogError("Entity is null.");
			Destroy(gameObject);
			return null;
		}

		Entity = entity;

		// Instantiate the model.
		// TODO: Need to clean up this part.
		GameObject go = GameObject.Instantiate(entity.Model, transform.Find("Graphics"));
		go.name = "Flesh Holder";
		go.transform.localPosition = Vector3.zero;

		// Let's load the Model Info.
		ModelInfo = go.GetComponent<ModelSetup>();
		if (ModelInfo == null)
		{
			Debug.LogWarning("Model " + go.name + " does not contain ModelSetup. Setting default values.");
			ModelInfo = go.AddComponent<ModelSetup>();
			ModelInfo.modelHeight = 1f;
			ModelInfo.dropables = new GameObject[0];
		}

		// Let's load the animator.
		Animator = go.GetComponent<Animator>();
		if (Animator == null)
		{
			Animator = go.AddComponent<Animator>();
		}
		Animator.Rebind();

		// Let's find the clicking area.
		ClickableTile = gameObject.GetComponentInChildren<ClickableArea>();
		if (ClickableTile == null)
		{
			Debug.LogError("Combat entity prefab does not contain Clicking Area.");
			Destroy(gameObject);
			return null;
		}
		AllowTargetable(false);

		// Let's set up the callback.
		if (OnDeathCallback != null)
		{
			OnDeath += OnDeathCallback;
		}

		// Let's set up the rest.
		Stats = entity.stats;
		StatusEffects = entity.statusEffects;

		return this;
	}

	public HealthEntity.Alignment GetFaction(TargetingData_Base.TargetAlignment alignment)
	{
		return Entity.alignment == HealthEntity.Alignment.Good ?
			(alignment == TargetingData_Base.TargetAlignment.Friendly ? HealthEntity.Alignment.Good : HealthEntity.Alignment.Evil) :
			(alignment == TargetingData_Base.TargetAlignment.Friendly ? HealthEntity.Alignment.Evil : HealthEntity.Alignment.Good);
	}

	public bool IsImmuneToDebuffs()
	{
		foreach (var statusEffect in StatusEffects)
		{
			foreach (var behaviour in statusEffect.data.behaviour)
			{
				if (behaviour is EmpowerBehaviour)
				{
					if (((EmpowerBehaviour)behaviour).immuneToDebuffs)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	public bool IsImmuneToDamage()
	{
		foreach (var statusEffect in StatusEffects)
		{
			foreach (var behaviour in statusEffect.data.behaviour)
			{
				if (behaviour is EmpowerBehaviour)
				{
					if (((EmpowerBehaviour)behaviour).immuneToDamage)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	public WeakenBehaviour[] GetCritMultiplierBehaviours()
	{
		List<WeakenBehaviour> behaviours = new List<WeakenBehaviour>();
		foreach (var statusEffect in StatusEffects)
		{
			foreach (var behaviour in statusEffect.data.behaviour)
			{
				if (behaviour is WeakenBehaviour weaken)
				{
					behaviours.Add(weaken);
				}
			}
		}

		return behaviours.ToArray();
	}

	public bool IsVulnerable()
	{
		foreach (var statusEffect in StatusEffects)
		{
			foreach (var behaviour in statusEffect.data.behaviour)
			{
				if (behaviour is VulnerableBehaviour)
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Method for figuring if the entity is targetable.
	/// </summary>
	/// <returns></returns>
	public bool IsTargetable()
	{
		return true;
	}

	public bool IsAbleToAct()
	{
		foreach (var statusEffect in StatusEffects)
		{
			foreach (var behaviour in statusEffect.data.behaviour)
			{
				if (behaviour is DisableBehaviour)
				{
					if (((DisableBehaviour)behaviour).disablesAction)
					{
						return false;
					}
				}
			}
		}

		return true;
	}

	public bool IsAbleToAttack()
	{
		foreach (var statusEffect in StatusEffects)
		{
			foreach (var behaviour in statusEffect.data.behaviour)
			{
				if (behaviour is DisableBehaviour)
				{
					if (((DisableBehaviour)behaviour).disablesAttacks)
					{
						return false;
					}
				}
			}
		}

		return true;
	}

	public bool IsAbleToCast()
	{
		foreach (var statusEffect in StatusEffects)
		{
			foreach (var behaviour in statusEffect.data.behaviour)
			{
				if (behaviour is DisableBehaviour)
				{
					if (((DisableBehaviour)behaviour).disablesSpells)
					{
						return false;
					}
				}
			}
		}

		return true;
	}

	public void AfflictWithStatusEffect(StatusEffectScriptableObject effectSO, int duration)
	{
		if (IsDead)
		{
			return;
		}

		if (IsImmuneToDebuffs())
		{
			if (effectSO.type == StatusEffectScriptableObject.Type.Disable || effectSO.type == StatusEffectScriptableObject.Type.Poison)
			{
				Animator.SetTrigger("Block");
				DamageNumberCanvas canva = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
				canva.Display("Immune", Color.magenta);
				canva.transform.position = transform.position;
				return;
			}
		}

		StatusEffect.TickTime tickTime = StatusEffect.TickTime.TurnStart;

		if (FindObjectOfType<CombatManager>().turn == CombatManager.TurnSelection.Enemies)
		{
			if (this is PlayerCombatObject)
			{
				tickTime = StatusEffect.TickTime.TurnEnd;
			}
		}

		StatusEffect effect = new StatusEffect(effectSO, tickTime, duration);
		StatusEffects.Add(effect);

		DamageNumberCanvas canv = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
		canv.Display("Applying: " + effect.data.name, Color.green);
		canv.transform.position = transform.position;

		foreach (var behaviour in effect.data.behaviour)
		{
			if (effect.tickTime == StatusEffect.TickTime.TurnStart)
			{
				OnStartTurn += behaviour.TickPerTurn;
			}
			else
			{
				OnEndTurn += behaviour.TickPerTurn;
			}
			OnAttackedBehaviour += behaviour.OnAttacked;
			behaviour.OnAfflicted(this);
		}

		RefreshUI();
	}

	public void RemoveStatusEffect(StatusEffect effect)
	{
		if (IsDead)
		{
			return;
		}

		foreach (var behaviour in effect.data.behaviour)
		{
			if (effect.tickTime == StatusEffect.TickTime.TurnStart)
			{
				OnStartTurn -= behaviour.TickPerTurn;
			}
			else
			{
				OnEndTurn -= behaviour.TickPerTurn;
			}
			OnAttackedBehaviour -= behaviour.OnAttacked;
			behaviour.OnUnafflicted(this);
		}

		DamageNumberCanvas canv = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
		canv.Display("Expired: " + effect.data.name, Color.red);
		canv.transform.position = transform.position;

		StatusEffects.Remove(effect);

		RefreshUI();
	}

	public virtual void OnTurnStart()
	{
		if (OnStartTurn != null)
		{
			OnStartTurn(this);
		}

		for (int i = StatusEffects.Count - 1; i > -1; i--)
		{
			StatusEffect effect = StatusEffects[i];

			if (effect.tickTime == StatusEffect.TickTime.TurnStart)
			{
				effect.durationRemaining--;

				if (effect.durationRemaining < 1)
				{
					RemoveStatusEffect(effect);
				}
			}
		}

		ResetBlock();
	}

	public virtual void OnTurnEnd()
	{
		if (OnEndTurn != null)
		{
			OnEndTurn(this);
		}

		for (int i = StatusEffects.Count - 1; i > -1; i--)
		{
			StatusEffect effect = StatusEffects[i];

			if (effect.tickTime == StatusEffect.TickTime.TurnEnd)
			{
				effect.durationRemaining--;

				if (effect.durationRemaining < 1)
				{
					RemoveStatusEffect(effect);
				}
			}
		}
	}

	public void TakeHealing(int amount)
	{
		if (IsDead)
		{
			return;
		}

		int startingHealth = Health;
		amount = Mathf.Clamp(amount, 0, amount);

		Entity.Health += amount;
		Entity.Health = Mathf.Clamp(Health, 0, MaxHealth);

		int actualAmountHealed = Health - startingHealth;

		if (actualAmountHealed > 0)
		{
			DamageNumberCanvas canv = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
			canv.Display("+" + actualAmountHealed.ToString(), Color.green);
			canv.transform.position = transform.position;
		}

		RefreshUI();
	}

	public DamageInfo TakeDamage(AbilityStep_Damage stepInfo, HealthEntityCombatObject attacker)
	{
		var result = DamageCalculator.CaclulateFlatDamage(stepInfo, attacker, this);

		/*
		if (IsDead)
		{
			return default(DamageInfo);
		}

		if (IsImmuneToDamage())
		{
			Animator.SetTrigger("Block");
			DamageNumberCanvas canv = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
			canv.Display("Immune", Color.magenta);
			canv.transform.position = transform.position;
			return default(DamageInfo);
		}
		*/
		DamageInfo damageInfo = new DamageInfo();/*

		int amount = stepInfo.amount;
		amount = Mathf.Max(0, amount);
		if (attacker != null)
		{
			amount = (int)(amount * attacker.Stats.rawDamageMultiplier.Value);
		}

		float random = Random.Range(0f, 1f);
		float attackerCritChance = attacker.Stats.criticalChance.Value;

		foreach (var statusEffect in StatusEffects)
		{
			foreach (var behaviour in statusEffect.behaviour)
			{
				if (behaviour is WeakenBehaviour weaken)
				{
					attackerCritChance *= weaken.modifyCritChanceAgainst;
				}
			}
		}

		bool crit = false;
		if (attacker != null && random < attackerCritChance && stepInfo.canCriticallyStrike)
		{
			crit = true;
			amount *= 2;
		}

		switch (stepInfo.damageTypePerc)
		{
			case AbilityStep_Damage.DamageTypePerc.PercentageMaxHP:
				amount = (int)(MaxHealth * (amount / 100f));
				break;
			case AbilityStep_Damage.DamageTypePerc.None:
				amount = 0;
				break;
		}

		switch (stepInfo.damageType)
		{
			case AbilityStep_Damage.DamageType.Physical:
				amount = (int)Mathf.Ceil(amount * (1 - Mathf.Clamp01(Stats.armor.Value - ((attacker != null) ? attacker.Stats.armorPenetration.Value : 0))));
				break;
			case AbilityStep_Damage.DamageType.Magic:
				amount = (int)Mathf.Ceil(amount * (1 - Mathf.Clamp01(Stats.magicResist.Value - ((attacker != null) ? attacker.Stats.magicPenetration.Value : 0))));
				break;
			case AbilityStep_Damage.DamageType.None:
				amount = 0;
				break;
		}*/

		//int amountRemaining = amount;

		switch (result.name)
		{
			case DamageCalculator.ClashResult.Name.Failed:
				return default;
			case DamageCalculator.ClashResult.Name.Immune:
				Animator.SetTrigger("Block");
				DamageNumberCanvas canv = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
				canv.Display("Immune", Color.magenta);
				canv.transform.position = transform.position;
				return default;
		}

		ReduceBlock(ref result.damage);

		/*
		amountRemaining -= Block;
		ModifyBlock(-amount);

		amountRemaining = Mathf.Clamp(amountRemaining, 0, amountRemaining);*/

		if (result.damage > 0)
		{
			damageInfo.damageDealt = result.damage;

			DamageNumberCanvas canv = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
			if (result.criticalHit)
			{
				canv.Display("CRIT! " + damageInfo.damageDealt.ToString(), Color.red);
			}
			else
			{
				canv.Display(damageInfo.damageDealt.ToString(), Color.red);
			}
			canv.transform.position = transform.position;

			Entity.Health -= damageInfo.damageDealt;
			Entity.Health = Mathf.Clamp(Health, 0, MaxHealth);

			if (Entity.Health > 0)
			{
				Animator.SetTrigger("Hit");
			}
		}
		else
		{
			Animator.SetTrigger("Block");
			DamageNumberCanvas canv = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
			canv.Display("Blocked", Color.grey);
			canv.transform.position = transform.position;
		}

		if (attacker != null)
		{
			if (OnAttackedBehaviour != null)
			{
				OnAttackedBehaviour(attacker);
			}
		}

		if (Health <= 0)
		{
			Die();
			OnDeath(this);
		}

		RefreshUI();

		return damageInfo;
	}

	protected virtual void Die()
	{
		IsDead = true;
		Entity.Health = 0;
		Animator.SetBool("Death", true);

		OnDeath(this);
	}

	public void InstaKill()
	{
		DamageNumberCanvas canv = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
		canv.Display("Death", Color.red);
		canv.transform.position = transform.position;
		Die();
	}

	public void ModifyBlock(int amount)
	{
		if (IsDead)
		{
			return;
		}

		if (amount > 0)
		{
			DamageNumberCanvas canv = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
			canv.Display("+" + amount.ToString(), Color.grey);
			canv.transform.position = transform.position;
		}

		Block += amount;
		Block = Mathf.Clamp(Block, 0, Block);

		RefreshUI();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="amount"></param>
	public void ReduceBlock(ref int strikeDamage)
	{
		if (IsDead || strikeDamage < 1)
		{
			return;
		}

		int startingDamage = strikeDamage;
		strikeDamage -= Block;
		
		if (strikeDamage < 0)
		{
			strikeDamage = 0;
			Block -= startingDamage;
		}
		else
		{
			Block = 0;
		}
	}

	public void AllowTargetable(bool allow)
	{
		if (IsDead)
		{
			ClickableTile.ShowArea(false);
			return;
		}

		ClickableTile.ShowArea(allow);
	}

	public void ResetBlock()
	{
		if (IsDead)
		{
			return;
		}

		Block = 0;
		RefreshUI();
	}

	public struct DamageInfo
	{
		public int damageDealt;
	}
}