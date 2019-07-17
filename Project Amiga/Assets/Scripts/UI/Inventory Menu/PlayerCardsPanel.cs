using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Dungeon
{
	public class PlayerCardsPanel : HUDPanel
	{
		public GameObject cardPrefab;
		public ScrollRect scrollView;
		public Text title;

		public void Display(Player player)
		{
			title.text = player.Name;

			foreach (Transform t in scrollView.content)
			{
				Destroy(t.gameObject);
			}

			foreach (var card in player.cards)
			{
				DisplayCard displayCard = Instantiate(cardPrefab, scrollView.content).GetComponent<DisplayCard>();
				displayCard.Display(card);
			}
		}
	}
}