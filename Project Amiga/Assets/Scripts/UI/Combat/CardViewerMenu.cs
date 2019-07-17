using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Combat
{
	public class CardViewerMenu : HUDPanel
	{
		public GameObject cardPrefab;
		public ScrollRect scrollView;

		void DisplayPile(List<Card> pile, string title)
		{
			foreach (Transform card in scrollView.content)
			{
				GameObject.Destroy(card.gameObject);
			}

			List<Card> actualPile = new List<Card>(pile);
			actualPile = new PlayerHand.Pile(pile, true).Cards;
			transform.Find("Title").GetComponent<Text>().text = title;
			Transform cardHolder = scrollView.content;

			foreach (var card in actualPile)
			{
				GameObject newGO = GameObject.Instantiate(cardPrefab, cardHolder);
				newGO.GetComponent<DisplayCard>().Display(card);
			}
		}

		public void DisplayDiscardPile(PlayerCombatObject player)
		{
			DisplayPile(player.hand.discardPile.Cards, player.Entity.Name + "'s Discard Pile (random order)");
		}

		public void DisplayDrawPile(PlayerCombatObject player)
		{
			DisplayPile(player.hand.drawPile.Cards, player.Entity.Name + "'s Draw Pile (random order)");
		}

		public void Input_BackButton()
		{
			gameObject.SetActive(false);
		}
	}
}