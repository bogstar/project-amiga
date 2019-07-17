using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatObject : HealthEntityCombatObject
{
	public PlayerHand hand;

	public PlayerUI ui;

	public int Mana { get; private set; }
	public int MaxMana { get; private set; }


	/// <summary>
	/// Spawn a model and initialize the combat entity.
	/// </summary>
	/// <param name="player"></param>
	/// <param name="hand"></param>
	/// <param name="ui"></param>
	public void Init(Player player, PlayerHand hand, GameObject ui, System.Action<HealthEntityCombatObject> OnDeathCallback)
	{
		// Pass the info to Health Entity.
		if (Init(player as HealthEntity, OnDeathCallback) == null)
		{
			return;
		}

		MaxMana = player.mana;
		IsDead = false;

		this.hand = hand;
		hand.Init(player.cards, this, RefreshUI);

		this.ui.drawPileCardNumber = ui.transform.Find("Draw Pile").Find("Count").GetComponent<Text>();
		ui.transform.Find("Draw Pile").GetComponent<Button>().onClick.AddListener(delegate { ((Amiga.UI.Combat.CombatDisplay)GameManager.Instance.CurrentSceneSettings).Input_DisplayDrawPile(this); });
		this.ui.discardPileCardNumber = ui.transform.Find("Discard Pile").Find("Count").GetComponent<Text>();
		ui.transform.Find("Discard Pile").GetComponent<Button>().onClick.AddListener(delegate { ((Amiga.UI.Combat.CombatDisplay)GameManager.Instance.CurrentSceneSettings).Input_DisplayDiscardPile(this); });
		this.ui.hand = hand;

		RefreshUI();
	}

	public void Revive()
	{
		IsDead = false;
		TakeHealing(1);
	}

	public void DropAllCards()
	{
		hand.DropAllCards();

		RefreshUI();
	}

	public void DropCard(CardGameObject card, bool stayUp)
	{
		hand.DropCard(card.GetComponent<CardInHand>(), stayUp);

		RefreshUI();
	}

	public void DrawCards(int maxCardForPlayers)
	{
		if (IsDead)
		{
			return;
		}

		hand.DrawCards(maxCardForPlayers);
	}

	protected override void Die()
	{
		base.Die();

		Mana = 0;
		DropAllCards();
		RefreshUI();
	}

	public void ModifyMana(int amount)
	{
		Mana += amount;

		if (amount > 0)
		{
			DamageNumberCanvas canv = GameObject.Instantiate(FindObjectOfType<CombatManager>().damageNumberCanvasPrefab).GetComponent<DamageNumberCanvas>();
			canv.Display("+" + amount.ToString(), Color.blue);
			canv.transform.position = transform.position;
		}

		RefreshUI();
	}

	public void ResetMana()
	{
		Mana = MaxMana;

		RefreshUI();
	}

	public override void OnTurnStart()
	{
		if (IsDead)
		{
			return;
		}

		ResetMana();

		base.OnTurnStart();

		hand.DrawCards(5);
		RefreshUI();
	}

	public override void RefreshUI()
	{
		ui.UpdateUI();
	}
}