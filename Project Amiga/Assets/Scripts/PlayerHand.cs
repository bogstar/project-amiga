using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerHand : MonoBehaviour
{
	public GameObject cardPrefab;
	//public DisplayCard displayCard;
	public Amiga.UI.Combat.NativePowerSelector nativePowerSelector;

	PlayerCombatObject player;

	public RectTransform drawPilePosition;
	public RectTransform discardPilePosition;
	public RectTransform selectPosition;

	public float intersectionAmount;

	public float cardWidth;

	RectTransform rectTransform;

	public Pile drawPile;
	public Pile discardPile;
	List<CardInHand> cardsInHand = new List<CardInHand>();

	CardInHand currentHover;
	CardInHand currentSelect;
	int currentSelectIndex = -1;
	int currentHoverIndex = -1;

	float timebetweencards = .15f;

	System.Action OnUIRefresh;


	void Awake()
	{
		rectTransform = (RectTransform)transform;
	}

	public void Init(List<Card> startCombatCards, PlayerCombatObject player, System.Action UIRefreshCallback)
	{
		drawPile = new Pile(startCombatCards, true);
		discardPile = new Pile();
		cardsInHand = new List<CardInHand>();
		OnUIRefresh += UIRefreshCallback;
		this.player = player;
		nativePowerSelector.Init(player);
	}

	public List<CardInHand> GetAllCards(List<RaycastResult> raycastResults)
	{
		List<CardInHand> cards = new List<CardInHand>();

		foreach (var r in raycastResults)
		{
			if (r.gameObject.GetComponent<CardInHand>() != null)
			{
				CardInHand cardInHand = r.gameObject.GetComponent<CardInHand>();

				if (!cardInHand.hoverable)
					continue;
				if (!cardInHand.active)
					continue;
				if (cardInHand.hand != this)
					continue;

				cards.Add(r.gameObject.GetComponent<CardInHand>());
			}
		}

		return cards;
	}

	List<CardInHand> GetAllCards()
	{
		PointerEventData pointer = new PointerEventData(EventSystem.current);
		pointer.position = Input.mousePosition;

		List<CardInHand> cards = new List<CardInHand>();

		List<RaycastResult> raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointer, raycastResults);

		foreach (var r in raycastResults)
		{
			if (r.gameObject.GetComponent<CardInHand>() != null)
			{
				if (!r.gameObject.GetComponent<CardInHand>().hoverable || !r.gameObject.GetComponent<CardInHand>().active)
					continue;
				cards.Add(r.gameObject.GetComponent<CardInHand>());
			}
		}

		return cards;
	}

	public CardInHand GetCardWithHighestOrder(List<CardInHand> allCards)
	{
		CardInHand card = null;

		int currentHighest = -1;

		foreach (var c in allCards)
		{
			if(c.orderInHand > currentHighest)
			{
				currentHighest = c.orderInHand;
				card = c;
			}
		}

		return card;
	}
	
	public void UnhoverOverCard()
	{
		if(currentHover != null)
		{
			currentHover = null;
			currentHoverIndex = -1;
		}
		RecalculateCardsPosition();
	}
	
	public void HoverOverCard(CardInHand card)
	{
		UnhoverOverCard();
		currentHover = card;
		currentHoverIndex = card.orderInHand - 1;
		RecalculateCardsPosition();
	}

	public void UnselectCard()
	{
		if(currentSelect != null)
		{
			currentSelect.transform.SetParent(transform);
			currentSelect.transform.SetSiblingIndex(currentSelect.orderInHand - 1);
			currentSelect.hoverable = true;
			currentSelect = null;
			currentSelectIndex = -1;
		}
	}

	public void SelectCard(CardInHand card)
	{
		UnselectCard();

		currentSelectIndex = card.orderInHand - 1;
		currentSelect = card;
		currentSelect.transform.SetSiblingIndex(100);
		currentSelect.hoverable = false;
		RecalculateCardsPosition();
	}

	public void UpdateXD(List<CardInHand> allCardsUnderMouse, out CardInHand selectedCard)
	{
		selectedCard = null;
		if (allCardsUnderMouse.Count < 1)
		{
			UnhoverOverCard();
			return;
		}

		CardInHand selectedCardOrHighestIndex = GetCardWithHighestOrder(allCardsUnderMouse);

		if (selectedCardOrHighestIndex.orderInHand - 1 != currentHoverIndex)
		{
			selectedCard = selectedCardOrHighestIndex;
			HoverOverCard(selectedCardOrHighestIndex);
		}
	}

	float GetAngleForLinearLength(float length, float radius)
	{
		return Mathf.Asin((length / 2) / radius) * Mathf.Rad2Deg * 2;
	}

	public void RecalculateCardsPosition()
	{
		float angleBetweenCards = 0;
		int cardNum = cardsInHand.Count;
		float chordLength = rectTransform.sizeDelta.x - cardWidth;
		float radius = chordLength * 2;
		bool moreCardsThanCanFit = false;

		float lengthForEachCard = cardWidth - intersectionAmount;

		if ((cardWidth - intersectionAmount) * cardNum > chordLength)
		{
			moreCardsThanCanFit = true;
			lengthForEachCard = chordLength / cardNum;
		}

		angleBetweenCards = GetAngleForLinearLength(lengthForEachCard, radius);
		float angleForOneFullCard = GetAngleForLinearLength(cardWidth - intersectionAmount, radius);

		int j = -cardsInHand.Count / 2;
		float offset = 0;

		if (cardsInHand.Count % 2 == 0)
		{
			offset = angleBetweenCards / 2;
		}

		for (int i = 0; i < cardsInHand.Count; i++)
		{
			float offset2 = 0;

			if (currentHoverIndex > -1)
			{
				float finalAngle = (angleForOneFullCard - angleBetweenCards);

				if (chordLength / cardWidth < 2)
				{
					finalAngle = angleBetweenCards;
				}

				if (i > currentHoverIndex)
				{
					offset2 += finalAngle;
				}
			}

			if (!moreCardsThanCanFit)
			{
				offset2 = 0;
			}

			float yOffset = radius;
			
			if(currentHoverIndex > -1)
			{
				if(currentHoverIndex == i)
				{
					yOffset += 20;
				}
			}

			float angle = -(j * angleBetweenCards + offset + offset2);
			float yPos = 0;
			float xPos = 0;

			yPos = Mathf.Sin((angle + 90) * Mathf.Deg2Rad) * yOffset - radius;
			xPos = Mathf.Cos((angle + 90) * Mathf.Deg2Rad) * yOffset;

			if(i != currentSelectIndex)
			{
				cardsInHand[i].StartChanging(new Vector3(xPos, yPos, 0), angle, 1f);
			}
			else
			{
				cardsInHand[i].transform.SetParent(transform.parent);
				cardsInHand[i].StartChanging(selectPosition.localPosition, 0, 1.5f);
			}

			j++;
		}
	}

	public void AddCardToHand(Card card)
	{
		Transform playerUI = transform.parent;
		CardInHand newCard = GameObject.Instantiate(cardPrefab, playerUI).GetComponent<CardInHand>();
	    
		newCard.card = card;
		newCard.active = true;
		
		newCard.gameObject.name = "Card " + card.name + " " + (cardsInHand.Count + 1);
		newCard.transform.localScale = Vector3.zero;
		newCard.transform.localPosition = drawPilePosition.localPosition;
		newCard.transform.SetParent(rectTransform);
		newCard.hand = this;
		newCard.hoverable = true;
		CardGameObject cardGO = newCard.GetComponent<CardGameObject>();
		
		cardGO.card = card;
		cardGO.cardButton.onClick.AddListener(delegate { FindObjectOfType<InputManager>().ButtonInput_SelectCard(cardGO, player); });

		cardsInHand.Add(newCard);

		newCard.GetComponent<CardInHand>().orderInHand = rectTransform.childCount;

		newCard.GetComponent<DisplayCard>().Display(cardGO);

		RecalculateCardsPosition();
	}

	void RecalculateCardPositions()
	{
		for (int i = 0; i < cardsInHand.Count; i++)
		{
			cardsInHand[i].orderInHand = i + 1;
		}
	}

	public void DiscardCard(CardInHand card, bool animate)
	{
		UnselectCard();
		card.active = false;
		card.hoverable = false;
		card.GetComponentInChildren<Button>().interactable = false;
		cardsInHand.Remove(card);

		if (animate)
		{
			StartCoroutine(ThrowToDiscard(card));
		}
		else
		{
			card.transform.SetParent(transform.parent);
			card.StartChanging(discardPilePosition.localPosition, 0, 0);
			StartCoroutine(DestroyCard(card.gameObject));
		}

		RecalculateCardPositions();
		RecalculateCardsPosition();
	}

	IEnumerator ThrowToDiscard(CardInHand card)
	{
		card.transform.SetParent(transform.parent);
		card.StartChanging(selectPosition.localPosition, 0, 1.5f);
		GameObject cardGO = card.gameObject;
		yield return new WaitForSeconds(1f);
		card.StartChanging(discardPilePosition.localPosition, 0, 0);
		StartCoroutine(DestroyCard(cardGO));
	}

	IEnumerator DestroyCard(GameObject card)
	{
		yield return new WaitForSeconds(1f);
		card.transform.SetParent(FindObjectOfType<InputManager>().GUI);
		GameObject.Destroy(card);
	}

	bool DrawCard(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			if (drawPile.Count < 1)
			{
				if (discardPile.Count < 1)
				{
					return false;
				}

				drawPile = new Pile(discardPile, true);
				discardPile = new Pile();
			}

			Card cardToDraw = drawPile.DrawNext();
			
			if (OnUIRefresh != null)
			{
				OnUIRefresh();
			}
			AddCardToHand(cardToDraw);
		}

		return true;
	}

	public void DropCard(CardGameObject card, bool stayUp)
	{
		DropCard(card.GetComponent<CardInHand>(), stayUp);
	}

	public void DropCard(int index, bool stayUp)
	{
		DropCard(cardsInHand[index], stayUp);
	}

	public void DropCard(CardInHand card, bool stayUp)
	{
		Card c = card.card;
		cardsInHand.Remove(card);
		discardPile.Add(c);

		DiscardCard(card, stayUp);
	}

	public void DropAllCards()
	{
		for (int i = cardsInHand.Count - 1; i > -1; i--)
		{
			DropCard(i, false);
		}
	}

	public void DrawCards(int maxCardForPlayers)
	{
		StartCoroutine(DrawCardsCoroutine(maxCardForPlayers));
	}

	IEnumerator DrawCardsCoroutine(int cardNumber)
	{
		for (int i = 0; i < cardNumber; i++)
		{
			if (!DrawCard(1))
			{
				break;
			}
			yield return new WaitForSeconds(timebetweencards);
		}
	}

	[System.Serializable]
	public class Pile
	{
		public List<Card> Cards { get; private set; }
		public int Count { get { return Cards != null ? Cards.Count : 0; } }

		public Pile(Pile pile, bool shuffle = false) : this(pile.Cards, shuffle)
		{
		}

		public Pile(List<Card> cards, bool shuffle = false)
		{
			Cards = new List<Card>(cards);
			if (shuffle)
			{
				Shuffle();
			}
		}

		public Pile()
		{
			Cards = new List<Card>();
		}

		public void Add(Card c)
		{
			Cards.Insert(0, c);
		}

		public void Remove(Card card)
		{
			if (!Cards.Contains(card))
			{
				return;
			}

			Cards.Remove(card);
		}

		public Card DrawNext()
		{
			if (Cards.Count < 1)
			{
				return null;
			}

			Card card = Cards[0];
			Cards.RemoveAt(0);
			return card;
		}

		public void Shuffle()
		{
			for (int i = 0; i < Count - 1; i++)
			{
				int randomIndex = Random.Range(i + 1, Count);
				Card tempCard = Cards[i];
				Cards[i] = Cards[randomIndex];
				Cards[randomIndex] = tempCard;
			}
		}
	}
}