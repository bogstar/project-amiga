using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
	public GameObject entityPrefab;
	public GameObject floatingPlayerUiPrefab;
	public GameObject floatingEnemyUiPrefab;

	[Header("References")]
	public Text infoText;
	public EnemyManager enemyManager;
	public GameObject particlePrefab;
	public GameObject playersLocation;
	public GameObject damageNumberCanvasPrefab;
	public PlayerHand[] hands;
	public GameObject[] uis;
	public AudioClip VictorySound;
	public AudioClip DefeatSound;

	[Header("Private Values Publicized")]
	public List<CardPlayerScriptableObject> cardsPerWin;
	public int goldPerWin;

	public List<PlayerCombatObject> players;
	public List<EnemyCombatObject> enemies;
	public List<HealthEntityCombatObject> AllEntities
	{
		get
		{
			List<HealthEntityCombatObject> entities = new List<HealthEntityCombatObject>();
			foreach (var p in players)
			{
				entities.Add(p);
			}
			foreach (var e in enemies)
			{
				entities.Add(e);
			}
			return entities;
		}
	}

	PlayerCombatObject currentCardOwner;
	public CardGameObject currentSelectedCard;

	public TurnSelection turn { get; private set; }

	public System.Action<bool> OnCombatEnd;

	List<Coroutine> coroutines = new List<Coroutine>();

	public bool targetFound;
	List<HealthEntityCombatObject> chosenTargets;
	public List<List<HealthEntityCombatObject>> targetsPerAbilityStep;
	List<HealthEntityCombatObject> entitiesToAddThisTurn = new List<HealthEntityCombatObject>();

	bool combatOver;


	void Start()
	{
		infoText.text = "";
	}

	public HealthEntityCombatObject SummonOnEnemySide(EnemyScriptableObject enemySO)
	{/*
		// This looks sloppy. Rework.
		List<EnemyScriptableObject> enemyList = new List<EnemyScriptableObject>();
		enemyList.Add(enemySO);
		Enemy enemy = enemyManager.SpawnEnemies(enemyList, OnEnemyDeath)[0];

		entitiesToAddThisTurn.Add(enemy);

		return enemy;*/
		return null;
	}

	HealthEntityCombatObject SpawnEntity(HealthEntity entity, Transform entityHolder, PlayerHand hand = null, GameObject ui = null)
	{
		GameObject newHealthEntityGO = GameObject.Instantiate(entityPrefab, entityHolder);
		newHealthEntityGO.name = entity.Name;

		if (entity is Player)
		{
			PlayerCombatObject player = newHealthEntityGO.gameObject.AddComponent<PlayerCombatObject>();
			player.ui = GameObject.Instantiate(floatingPlayerUiPrefab, newHealthEntityGO.transform).GetComponent<PlayerUI>();
			player.ui.gameObject.name = "UI";
			player.ui.player = player;
			player.Init((Player)entity, hand, ui, OnEntityDeath);

			return player;
		}
		else if(entity is Enemy)
		{
			Enemy enemyEntity = entity as Enemy;
			EnemyCombatObject enemy = newHealthEntityGO.gameObject.AddComponent<EnemyCombatObject>();
			enemy.ui = GameObject.Instantiate(floatingEnemyUiPrefab, newHealthEntityGO.transform).GetComponent<EnemyUI>();
			enemy.ui.gameObject.name = "UI";
			enemy.ui.enemy = enemy;
			enemy.Init(enemyEntity, 0, OnEntityDeath);

			return enemy;
		}
		return null;
	}

	HealthEntityCombatObject SpawnEntity(HealthEntityScriptableObject entitySO, Transform entityHolder)
	{
		if(entitySO is PlayerCharacterScriptableObject)
		{
			PlayerCharacterScriptableObject playerSO = entitySO as PlayerCharacterScriptableObject;
			Player player = new Player(playerSO);
			return SpawnEntity(player, entityHolder);
		}
		else if(entitySO is EnemyScriptableObject)
		{
			EnemyScriptableObject enemySO = entitySO as EnemyScriptableObject;
			Enemy enemy = new Enemy(enemySO);
			return SpawnEntity(enemy, entityHolder);
		}
		return null;
	}

	public EncounterScriptableObject currentEncounter;

	public void StartCombat(EncounterScriptableObject encounterSO, List<Player> players, System.Action<bool> callback)
	{
		//List<EnemyScriptableObject> enemyList = new List<EnemyScriptableObject>();
		int i = 0;
		foreach (var enemy in encounterSO.enemies)
		{
			EnemyCombatObject newEnemy = SpawnEntity(enemy, enemyManager.enemyPanel) as EnemyCombatObject;
			newEnemy.enemyIndex = this.enemies.Count;
			newEnemy.transform.localPosition = new Vector3(-encounterSO.enemies.Length / 2 + i, 0f, 0f);
			newEnemy.transform.localEulerAngles = new Vector3(0, 180f, 0);
			this.enemies.Add(newEnemy);
			i++;
		}
		enemyManager.SpawnEnemies(this.enemies, OnEnemyDeath);

		currentEncounter = encounterSO;

		combatOver = false;

		i = 0;
		foreach (var p in players)
		{
			if (p.Health < 1)
			{
				p.Health = 1;
			}

			PlayerCombatObject newPlayer = SpawnEntity(p, playersLocation.transform, hands[i], uis[i]) as PlayerCombatObject;
			newPlayer.transform.localPosition = new Vector3(-1 + i * 2, 0, 0);
			this.players.Add(newPlayer);
			i++;
		}

		foreach (var p in this.players)
		{
			p.RefreshUI();
		}

		OnCombatEnd += callback;

		turn = TurnSelection.Players;

		StartTurn();
	}

	void StartTurn()
	{
		switch (turn)
		{
			case TurnSelection.Players:
				foreach (var p in players)
				{
					p.OnTurnStart();
				}
				foreach (var e in enemies)
				{
					e.ChooseNextAbility();
				}
				break;
			case TurnSelection.Enemies:
				for (int i = enemies.Count-1; i > -1; i--)
				{
					enemies[i].OnTurnStart();
				}
				EnemyTurnCoroutine = StartCoroutine(DoEnemyTurn());
				break;
		}
	}

	Coroutine EnemyTurnCoroutine = null;

	IEnumerator DoEnemyTurn()
	{
		foreach (var e in enemies)
		{
			bool doing = false;
			e.SetTargetsPerAbilityStep(SelectAbilityTargetEnemy(e));
			do
			{
				e.DoTurn(ref doing);
				yield return null;
			}
			while (doing == true);
		}

		EndTurn();
		yield return null;
	}

	void EndTurn()
	{
		switch (turn)
		{
			case TurnSelection.Players:
				foreach (var p in players)
				{
					p.OnTurnEnd();
				}
				turn = TurnSelection.Enemies;
				break;
			case TurnSelection.Enemies:
				foreach (var e in enemies)
				{
					e.OnTurnEnd();
				}
				turn = TurnSelection.Players;
				break;
		}

		foreach (var entity in entitiesToAddThisTurn)
		{
			enemies.Add(entity as EnemyCombatObject);
		}
		entitiesToAddThisTurn.Clear();

		StartTurn();
	}

	void DoPowerTrick()
	{
		currentCardOwner.Animator.SetTrigger("Casting");

		for (int i = 0; i < currentPower.abilityStepsWithTargetingData.Length; i++)
		{
			if (currentPower.abilityStepsWithTargetingData[i].abilityStep is AbilityStep_Drain drain)
			{
				if (i != 0)
				{
					drain.OnPlay(targetsPerAbilityStep[i], currentSelectedCard.card.onPlayResults[i - i]);
				}
			}
			else
			{
				AbilityStep_Base card = currentPower.abilityStepsWithTargetingData[i].abilityStep;

				card.OnPlay(targetsPerAbilityStep[i], currentCardOwner);
			}
		}

		FindObjectOfType<AudioManager>().PlayRandomClip(currentPower.soundsForPlay);

		currentCardOwner.ModifyMana(-currentPower.manaCost);
		//currentCardOwner.DropCard(currentSelectedCard, stayUp);

		infoText.text = "";
	}

	void DoCardTrick()
	{
		currentCardOwner.Animator.SetTrigger("Casting");

		for (int i = 0; i < currentSelectedCard.card.abilityStepsWithTargetingData.Length; i++)
		{
			if (currentSelectedCard.card.abilityStepsWithTargetingData[i].abilityStep is AbilityStep_Drain)
			{
				AbilityStep_Drain card = currentSelectedCard.card.abilityStepsWithTargetingData[i].abilityStep as AbilityStep_Drain;

				if (i != 0)
				{
					card.OnPlay(targetsPerAbilityStep[i], currentSelectedCard.card.onPlayResults[i - i]);
				}
			}
			else
			{
				AbilityStep_Base card = currentSelectedCard.card.abilityStepsWithTargetingData[i].abilityStep;

				currentSelectedCard.card.onPlayResults[i] = card.OnPlay(targetsPerAbilityStep[i], currentCardOwner);
			}
		}
		
		FindObjectOfType<AudioManager>().PlayRandomClip(currentSelectedCard.card.soundForPlay);
		
		currentCardOwner.ModifyMana(-currentSelectedCard.card.manaCost);
		currentCardOwner.DropCard(currentSelectedCard, stayUp);

		infoText.text = "";
	}

	bool stayUp;

	public void SelectTarget(HealthEntityCombatObject entity)
	{
		// Break if there is no selected card.
		if (currentSelectedCard == null)
		{
			return;
		}

		//HideTargettingButtons();
		chosenTargets.Add(entity);
		targetFound = true;
	}

	public struct TargetData
	{
		public bool targetDecided;
		public List<HealthEntityCombatObject> entities;
		public int previousStepTargets;
		public bool multipleSpecificMode;
		public int multipleSpecificNumber;
	}

	TargetData GetValidTargets(HealthEntityCombatObject caster, TargetingData_Base targetingData)
	{
		TargetData targetData = new TargetData();
		targetData.previousStepTargets = -1;
		List<HealthEntityCombatObject> targets = new List<HealthEntityCombatObject>();

		if (targetingData.targetingType == TargetingData_Base.TargetingType.NoTargeting)
		{
			// There is no targeting. Don't bother.
		}
		// We will use other step's targets.
		else if (targetingData.targetingType == TargetingData_Base.TargetingType.PreviousStep)
		{
			targetData.previousStepTargets = targetingData.useTargetsFromStepNumber;
		}
		// If the target is only self, do not check for anything else.
		else if (targetingData.onlySelf)
		{
			targets.Add(caster);
			targetData.targetDecided = true;
		}
		else
		{
			// Can we add self?
			bool selfIncluded = targetingData.selfIncluded;

			// Let's skip the check if we can only target ourselves.
			if (targetingData.onlySelf)
			{
				targets.Add(caster);
			}
			// We can target more than ourselves. Let's first check if we can target at all.
			else if (targetingData.targets != TargetingData_Base.Target.All)
			{
				switch (targetingData.targetAlignment)
				{
					case TargetingData_Base.TargetAlignment.Both:
						switch (targetingData.targets)
						{
							case TargetingData_Base.Target.One:
								// Select one from friendlies and from enemies.
								foreach (var f in GetEntitiesWithRelationTo(caster, TargetingData_Base.TargetAlignment.Both, selfIncluded))
								{
									targets.Add(f);
								}
								break;
							case TargetingData_Base.Target.MultipleSpecific:
								// Select multiple specific from friendlies or enemies.

								// Add ourselves to the list.
								if (targetingData.selfIncluded)
								{

								}
								break;
						}
						break;
					case TargetingData_Base.TargetAlignment.Friendly:
						switch (targetingData.targets)
						{
							case TargetingData_Base.Target.One:
								// Select one from friendlies taking into account whether to add self.
								foreach (var f in GetEntitiesWithRelationTo(caster, TargetingData_Base.TargetAlignment.Friendly, selfIncluded))
								{
									targets.Add(f);
								}
								break;
							case TargetingData_Base.Target.MultipleSpecific:
								// Select multiple specific friendlies taking into account whether to add self.
								break;
							case TargetingData_Base.Target.Multiple:
								// Select multiple friendlies taking into account whether to add self.
								foreach (var f in GetEntitiesWithRelationTo(caster, TargetingData_Base.TargetAlignment.Friendly, selfIncluded))
								{
									targets.Add(f);
								}
								break;
						}
						break;
					case TargetingData_Base.TargetAlignment.Hostile:
						switch (targetingData.targets)
						{
							case TargetingData_Base.Target.One:
								// Select one from enemies.
								foreach (var e in GetEntitiesWithRelationTo(caster, TargetingData_Base.TargetAlignment.Hostile))
								{
									targets.Add(e);
								}
								break;
							case TargetingData_Base.Target.MultipleSpecific:
								// Select multiple specific from enemies.
								targetData.multipleSpecificMode = true;
								targetData.multipleSpecificNumber = targetingData.numberOfTargets;
								foreach (var e in GetEntitiesWithRelationTo(caster, TargetingData_Base.TargetAlignment.Hostile))
								{
									targets.Add(e);
								}
								break;
							case TargetingData_Base.Target.Multiple:
								// Select multiple from enemies.
								foreach (var e in GetEntitiesWithRelationTo(caster, TargetingData_Base.TargetAlignment.Hostile))
								{
									targets.Add(e);
								}
								break;
						}
						break;
				}
			}
			// We cannot target so we choose all.
			else
			{
				switch (targetingData.targetAlignment)
				{
					case TargetingData_Base.TargetAlignment.Both:
						// Select all targets.
						foreach (var a in GetEntitiesWithRelationTo(caster, TargetingData_Base.TargetAlignment.Both, selfIncluded))
						{
							targets.Add(a);
						}
						break;
					case TargetingData_Base.TargetAlignment.Friendly:
						// Select all allies taking into account if self included
						foreach (var a in GetEntitiesWithRelationTo(caster, TargetingData_Base.TargetAlignment.Friendly, selfIncluded, false, targetingData.canTargetDead))
						{
							targets.Add(a);
						}
						break;
					case TargetingData_Base.TargetAlignment.Hostile:
						// Select all enemies.
						foreach (var e in GetEntitiesWithRelationTo(caster, TargetingData_Base.TargetAlignment.Hostile))
						{
							targets.Add(e);
						}
						break;
				}
				targetData.targetDecided = true;
			}
		}

		targetData.entities = targets;

		return targetData;
	}

	void ShowTargetingButton(List<HealthEntityCombatObject> entities)
	{
		foreach (var e in entities)
		{
			if(e is PlayerCombatObject)
				((PlayerCombatObject)e).AllowTargetable(true);
			if(e is EnemyCombatObject)
				((EnemyCombatObject)e).AllowTargetable(true);
		}
	}

	List<List<HealthEntityCombatObject>> SelectAbilityTargetEnemy(EnemyCombatObject enemy)
	{
		List<List<HealthEntityCombatObject>> targetsPerAbilityStep = new List<List<HealthEntityCombatObject>>();

		AbilityStepsWithTargetingData_Enemy[] abilityStepsWithTargetingData = enemy.NextAbility.abilityStepsWithTargetingData;

		// Let's cycle through all ability steps in order to determine targets.
		foreach (var abilityStepWithTargetingData in abilityStepsWithTargetingData)
		{
			List<HealthEntityCombatObject> selectedTargets = new List<HealthEntityCombatObject>();

			TargetData targetData = GetValidTargets(enemy, abilityStepWithTargetingData.targetingData);

			if(targetData.previousStepTargets > -1)
			{
				foreach(var entity in targetsPerAbilityStep[targetData.previousStepTargets])
				{
					selectedTargets.Add(entity);
				}
			}
			else if (targetData.targetDecided)
			{
				foreach (var e in targetData.entities)
				{
					selectedTargets.Add(e);
				}
			}
			else
			{
				switch (abilityStepWithTargetingData.targetingData.targets)
				{
					case TargetingData_Base.Target.One:
						// Choose a random target for now
						selectedTargets.Add(targetData.entities[Random.Range(0, targetData.entities.Count)]);
						break;
					case TargetingData_Base.Target.Multiple:
						// Choose a random target for now
						selectedTargets.Add(targetData.entities[Random.Range(0, targetData.entities.Count)]);
						break;
					case TargetingData_Base.Target.MultipleSpecific:
						// Choose a random target for now

						// If it happens that there are less available targets than our ability wants us to target,
						// add them all.
						if (abilityStepWithTargetingData.targetingData.numberOfTargets < targetData.entities.Count)
						{
							foreach (var entity in targetData.entities)
							{
								selectedTargets.Add(entity);
							}
						}
						else
						{
							List<HealthEntityCombatObject> st = new List<HealthEntityCombatObject>(selectedTargets);
							for (int i = 0; i < abilityStepWithTargetingData.targetingData.numberOfTargets; i++)
							{
								HealthEntityCombatObject entity = targetData.entities[Random.Range(0, targetData.entities.Count)];
								selectedTargets.Add(entity);
								st.Remove(entity);
							}
						}
						break;
				}
			}

			targetsPerAbilityStep.Add(selectedTargets);
		}

		// We have all chosen targets.
		// We now have to check whether any of those are multiple hits.
		for (int i = 0; i < targetsPerAbilityStep.Count; i++)
		{
			TargetingData_Base targetingData = enemy.NextAbility.abilityStepsWithTargetingData[i].targetingData;

			if (targetingData.targetAlignment == TargetingData_Base.TargetAlignment.Friendly)
			{
				if (targetingData.targets == TargetingData_Base.Target.Multiple)
				{
					EnemyCombatObject chosenTarget = (EnemyCombatObject)targetsPerAbilityStep[i][0];
					List<EnemyCombatObject> nearbyEnemies = GetNearbyEnemies(chosenTarget, targetingData.numberOfTargets);
					foreach (var e in nearbyEnemies)
					{
						targetsPerAbilityStep[i].Add(e);
					}
				}
			}
		}

		return targetsPerAbilityStep;
	}

	public void AddTarget(HealthEntityCombatObject entity, System.Action HideTargetingButtonsCallback)
	{
		chosenTargets.Add(entity);
		if (!multipleSpecificMode)
		{
			targetFound = true;
			HideTargetingButtonsCallback();
		}
		else
		{
			multipleSpecificModeList.Add(entity);
			if(multipleSpecificModeList.Count == numberOfSpecificTargets)
			{
				targetFound = true;
				HideTargetingButtonsCallback();
			}
		}
	}

	public bool multipleSpecificMode;
	public int numberOfSpecificTargets;
	public List<HealthEntityCombatObject> multipleSpecificModeList;
	public int stepNumber;

	IEnumerator SelectTargets(CardGameObject cardGO, System.Action<CardInHand> SelectCardCallback, System.Action RecalculateCardsPositionCallback)
	{
		PlayerCombatObject playerOnTurn = currentCardOwner;
		HealthEntityCombatObject caster = playerOnTurn;
		stepNumber = 0;

		targetsPerAbilityStep = new List<List<HealthEntityCombatObject>>();

		AbilityStepsWithTargetingData_Player[] abilities = cardGO.card.abilityStepsWithTargetingData;

		// Let's cycle through all ability steps in order to determine targets.
		foreach (var abilityStepWithTargetingData in abilities)
		{
			chosenTargets = new List<HealthEntityCombatObject>();
			targetFound = false;

			TargetData targetData = GetValidTargets(caster, abilityStepWithTargetingData.targetingData);
			if (targetData.multipleSpecificMode)
			{
				multipleSpecificMode = true;
				numberOfSpecificTargets = targetData.multipleSpecificNumber;
				multipleSpecificModeList = new List<HealthEntityCombatObject>();
			}
			else
			{
				multipleSpecificMode = false;
			}

			if (targetData.previousStepTargets > -1)
			{
				targetFound = true;
				foreach (var entity in targetsPerAbilityStep[targetData.previousStepTargets])
				{
					chosenTargets.Add(entity);
				}
			}
			else if (targetData.targetDecided)
			{
				targetFound = true;
				foreach (var e in targetData.entities)
				{
					chosenTargets.Add(e);
				}
			}
			else
			{
				if (targetData.entities.Count == 1 && targetData.entities[0] is PlayerCombatObject)
				{
					targetFound = true;
					chosenTargets.Add(targetData.entities[0]);
				}
				else
				{
					SelectCardCallback(cardGO.GetComponent<CardInHand>());
					RecalculateCardsPositionCallback();
					infoText.text = abilityStepWithTargetingData.targetingData.textForChoosing;
					ShowTargetingButton(targetData.entities);
				}
			}

			while (targetFound == false)
			{
				yield return null;
			}

			stepNumber++;
			targetsPerAbilityStep.Add(chosenTargets);
		}

		// We have all chosen targets.
		// We now have to check whether any of those are multiple hits.

		for (int i = 0; i < targetsPerAbilityStep.Count; i++)
		{
			TargetingData_Base targetingData = cardGO.card.abilityStepsWithTargetingData[i].targetingData;

			if (targetingData.targetAlignment == TargetingData_Base.TargetAlignment.Hostile)
			{
				if (targetingData.targets == TargetingData_Base.Target.Multiple)
				{
					EnemyCombatObject chosenTarget = (EnemyCombatObject)targetsPerAbilityStep[i][0];
					List<EnemyCombatObject> nearbyEnemies = GetNearbyEnemies(chosenTarget, targetingData.numberOfTargets);
					foreach (var e in nearbyEnemies)
					{
						targetsPerAbilityStep[i].Add(e);
					}
				}
			}
		}

		// Now we have full lists of all targets for each attribute.
		// Let's do the card trick.
	
		DoCardTrick();

		yield return null;
	}

	IEnumerator SelectTargets_Power(NativePowerScriptableObject nativePower)
	{
		PlayerCombatObject playerOnTurn = currentCardOwner;
		HealthEntityCombatObject caster = playerOnTurn;
		stepNumber = 0;

		targetsPerAbilityStep = new List<List<HealthEntityCombatObject>>();

		AbilityStepsWithTargetingData_Player[] abilities = nativePower.abilityStepsWithTargetingData;

		// Let's cycle through all ability steps in order to determine targets.
		foreach (var abilityStepWithTargetingData in abilities)
		{
			chosenTargets = new List<HealthEntityCombatObject>();
			targetFound = false;

			TargetData targetData = GetValidTargets(caster, abilityStepWithTargetingData.targetingData);
			if (targetData.multipleSpecificMode)
			{
				multipleSpecificMode = true;
				numberOfSpecificTargets = targetData.multipleSpecificNumber;
				multipleSpecificModeList = new List<HealthEntityCombatObject>();
			}
			else
			{
				multipleSpecificMode = false;
			}

			if (targetData.previousStepTargets > -1)
			{
				targetFound = true;
				foreach (var entity in targetsPerAbilityStep[targetData.previousStepTargets])
				{
					chosenTargets.Add(entity);
				}
			}
			else if (targetData.targetDecided)
			{
				targetFound = true;
				foreach (var e in targetData.entities)
				{
					chosenTargets.Add(e);
				}
			}
			else
			{
				if (targetData.entities.Count == 1 && targetData.entities[0] is PlayerCombatObject)
				{
					targetFound = true;
					chosenTargets.Add(targetData.entities[0]);
				}
				else
				{
					infoText.text = abilityStepWithTargetingData.targetingData.textForChoosing;
					ShowTargetingButton(targetData.entities);
				}
			}

			while (targetFound == false)
			{
				yield return null;
			}

			stepNumber++;
			targetsPerAbilityStep.Add(chosenTargets);
		}

		// We have all chosen targets.
		// We now have to check whether any of those are multiple hits.

		for (int i = 0; i < targetsPerAbilityStep.Count; i++)
		{
			TargetingData_Base targetingData = nativePower.abilityStepsWithTargetingData[i].targetingData;

			if (targetingData.targetAlignment == TargetingData_Base.TargetAlignment.Hostile)
			{
				if (targetingData.targets == TargetingData_Base.Target.Multiple)
				{
					EnemyCombatObject chosenTarget = (EnemyCombatObject)targetsPerAbilityStep[i][0];
					List<EnemyCombatObject> nearbyEnemies = GetNearbyEnemies(chosenTarget, targetingData.numberOfTargets);
					foreach (var e in nearbyEnemies)
					{
						targetsPerAbilityStep[i].Add(e);
					}
				}
			}
		}

		// Now we have full lists of all targets for each attribute.
		// Let's do the card trick.

		DoPowerTrick();

		yield return null;
	}

	/// <summary>
	/// Get all entities in combat with the relationship as alignment to the reference entity.
	/// </summary>
	/// <param name="entity">The reference entity</param>
	/// <param name="alignment">Relationship with the required entity</param>
	/// <param name="targetSelf">Can target itself?</param>
	/// <param name="canTargetUntargetable">Can target untargetable targets?</param>
	/// <returns></returns>
	List<HealthEntityCombatObject> GetEntitiesWithRelationTo(HealthEntityCombatObject entity, TargetingData_Base.TargetAlignment alignment, bool targetSelf = false, bool canTargetUntargetable = false, bool canTargetDead = false)
	{
		List<HealthEntityCombatObject> entities = new List<HealthEntityCombatObject>();

		foreach (var e in AllEntities)
		{
			switch (alignment)
			{
				case TargetingData_Base.TargetAlignment.Both:
					if (ValidTarget(entity, canTargetUntargetable, canTargetDead))
					{
						entities.Add(e);
					}
					break;
				default:
					if (e.GetFaction(alignment) == entity.alignment && (targetSelf && e == entity || e != entity) && ValidTarget(entity, canTargetUntargetable, canTargetDead))
					{
						entities.Add(e);
					}
					break;
			}
		}

		return entities;
	}

	/// <summary>
	/// Figure out if the entity is a valid target regarding some arguments.
	/// </summary>
	/// <param name="entity">Referenced entity</param>
	/// <param name="canTargetUntargetable">Bypass the untargetable restriction</param>
	/// <param name="canTargetDead">Bypass the dead restriction</param>
	/// <returns></returns>
	bool ValidTarget(HealthEntityCombatObject entity, bool canTargetUntargetable, bool canTargetDead)
	{
		// If dead
		if (entity.IsDead && canTargetDead)
		{
			return true;
		}
		// If NOT dead
		else if (!entity.IsDead)
		{
			if (canTargetUntargetable)
			{
				return true;
			}
			else if (entity.IsTargetable())
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Returns list with nearby enemies to the referenced enemy. Can only calculate for odd numbers.
	/// </summary>
	/// <param name="enemy">Referenced enemy</param>
	/// <param name="enemyNumber">Number of enemies</param>
	/// <returns></returns>
	public List<EnemyCombatObject> GetNearbyEnemies(EnemyCombatObject enemy, int enemyNumber)
	{
		if (enemyNumber % 2 == 0)
		{
			Debug.LogError("Only odd numbers for now.");
			return null;
		}
		if (enemyNumber < 3)
		{
			Debug.LogError("No point calculating this for less than 3 enemies.");
			return null;
		}

		List<EnemyCombatObject> nearbyEnemies = new List<EnemyCombatObject>();
		List<EnemyCombatObject> enemiesAsEnemy = new List<EnemyCombatObject>();

		foreach (var entity in enemies)
		{
			enemiesAsEnemy.Add(entity as EnemyCombatObject);
		}

		int enemiesRemaining = enemies.Count;
		int enemyIndex = enemy.enemyIndex;

		if (enemiesRemaining <= enemyNumber)
		{
			nearbyEnemies = new List<EnemyCombatObject>(enemiesAsEnemy);
		}
		else
		{
			if (enemyIndex < enemyNumber / 2 + 1)
			{
				for (int i = 0; i < enemyNumber; i++)
				{
					nearbyEnemies.Add(enemiesAsEnemy[i]);
				}
			}
			else if (enemyIndex > (enemiesRemaining - enemyNumber))
			{
				for (int i = enemiesRemaining - enemyNumber; i < enemiesRemaining; i++)
				{
					nearbyEnemies.Add(enemiesAsEnemy[i]);
				}
			}
			else
			{
				int startingIndex = enemyNumber / 2;
				for (int i = -startingIndex; i <= startingIndex; i++)
				{
					nearbyEnemies.Add(enemiesAsEnemy[enemyIndex + i]);
				}
			}
		}

		nearbyEnemies.Remove(enemy);
		return nearbyEnemies;
	}

	public void SelectNativePower(NativePowerScriptableObject nativePower, PlayerCombatObject player)
	{
		// Player has selected a native power.

		// Ignore if it is not player's turn.
		if (turn != TurnSelection.Players)
		{
			infoText.text = "It's not players' turn.";
			return;
		}

		// From here on, it's player's turn.

		// Ignore if the player is dead.
		if (player.IsDead)
		{
			infoText.text = "Player is knocked out.";
			return;
		}

		// Ignore if the player is disabled.
		if (!player.IsAbleToAct())
		{
			infoText.text = "Player cannot act.";
			return;
		}

		// Ignore if the player does not have enough mana to activate the power.
		if (nativePower.manaCost > player.Mana)
		{
			infoText.text = "Not enough mana.";
			return;
		}

		infoText.text = "";
		currentCardOwner = player;
		currentPower = nativePower;

		foreach (var c in coroutines)
		{
			StopCoroutine(c);
		}

		StartCoroutine(SelectTargets_Power(nativePower));
	}

	NativePowerScriptableObject currentPower;

	public void SelectCard(CardGameObject cardGO, PlayerCombatObject player, System.Action<CardInHand> SelectCardCallback, System.Action RecalculateCardsPositionCallback)
	{
		// Player has selected a card from the hand.

		// Ignore if it is not player's turn.
		if (turn != TurnSelection.Players)
		{
			infoText.text = "It's not players' turn.";
			return;
		}

		// From here on, it's player's turn.

		// Ignore if the player is dead.
		if (player.IsDead)
		{
			infoText.text = "Player is knocked out.";
			return;
		}

		// Ignore if the player is disabled.
		if (!player.IsAbleToAct())
		{
			infoText.text = "Player cannot act.";
			return;
		}

		// Ignore if the player is disarmed and the card is attack.
		if (cardGO.card.data.type == CardPlayerScriptableObject.Type.Attack && !player.IsAbleToAttack())
		{
			infoText.text = "Player is disarmed.";
			return;
		}

		// Ignore if the player is silenced and the card is spell.
		if (cardGO.card.data.type == CardPlayerScriptableObject.Type.Spell && !player.IsAbleToCast())
		{
			infoText.text = "Player is silenced.";
			return;
		}

		// Ignore if the player does not have enough mana to play the card.
		if (cardGO.card.manaCost > player.Mana)
		{
			infoText.text = "Not enough mana.";
			return;
		}

		infoText.text = "";
		stayUp = true;
		currentCardOwner = player;
		currentSelectedCard = cardGO;

		foreach (var c in coroutines)
		{
			StopCoroutine(c);
		}

		coroutines.Add(StartCoroutine(SelectTargets(cardGO, SelectCardCallback, RecalculateCardsPositionCallback)));
	}

	public void PlayerEndTurn()
	{
		if (turn != TurnSelection.Players)
		{
			return;
		}

		foreach (var player in players)
		{
			player.AllowTargetable(false);
			player.hand.UnselectCard();
			player.DropAllCards();
		}

		foreach (var enemy in enemies)
		{
			enemy.AllowTargetable(false);
		}

		EndTurn();
	}

	IEnumerator EndBattleCoroutine(bool defeat)
	{
		foreach (var player in players)
		{
			for (int i = player.StatusEffects.Count - 1; i > -1; i--)
			{
				player.RemoveStatusEffect(player.StatusEffects[i]);
			}
			player.RefreshUI();
		}
		yield return new WaitForSeconds(3f);

		if (defeat)
		{
			OnCombatEnd(defeat);
		}
		else
		{
			((Amiga.UI.Combat.CombatDisplay)GameManager.Instance.CurrentSceneSettings).DisplaySpoilsMenu();
		}
	}

	void OnEntityDeath(HealthEntityCombatObject entity)
	{
		if (entity is PlayerCombatObject)
		{
			bool allPlayersDead = true;
			foreach (var player in players)
			{
				if (!player.IsDead)
				{
					allPlayersDead = false;
					break;
				}
			}
			if (allPlayersDead)
			{
				FindObjectOfType<AudioManager>().PlayClip(DefeatSound);
				StopCoroutine(EnemyTurnCoroutine);
				combatOver = true;
				StartCoroutine(EndBattleCoroutine(true));
			}
		}
		else if (entity is EnemyCombatObject)
		{
			OnEnemyDeath(entity);
		}
	}

	void OnEnemyDeath(HealthEntityCombatObject entity)
	{
		EnemyCombatObject enemy = entity as EnemyCombatObject;
		FindObjectOfType<AudioManager>().PlayRandomClip(enemy.deathSounds);

		enemies.Remove(enemy);
		int i = 0;
		foreach (var e in enemies)
		{
			e.enemyIndex = i;
			i++;
		}
		if (enemies.Count == 0)
		{
			FindObjectOfType<AudioManager>().PlayClip(VictorySound);
			combatOver = true;
			StartCoroutine(EndBattleCoroutine(false));
		}
	}

	public void ManualInput_Victory()
	{
		if (combatOver)
		{
			return;
		}
		enemyManager.KillAllEnemies(enemies);
	}

	public enum TurnSelection { None, Players, Enemies }
}
