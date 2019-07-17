using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Combat
{
	public class CombatDisplay : SceneSettings
	{
		[Header("References")]
		[SerializeField]
		GameObject battleHolder;
		[SerializeField]
		CardViewerMenu cardViewerMenu;
		[SerializeField]
		SpoilsMenu spoilsMenu;
		[SerializeField]
		Amiga.UI.PauseMenu.PauseMenu pausePanel;

		void Start()
		{
			Instantiate(versionPrefab, battleHolder.transform);
		}

		public void DisplaySpoilsMenu()
		{
			ShowPanel(spoilsMenu);
			spoilsMenu.Display(FindObjectOfType<CombatManager>().currentEncounter);
		}

		public void Input_CloseSpoilsMenu()
		{
			FindObjectOfType<CombatManager>().OnCombatEnd(false);
		}

		public void Input_DisplayDrawPile(PlayerCombatObject p)
		{
			ShowPanel(cardViewerMenu);
			cardViewerMenu.DisplayDrawPile(p);
		}

		public void Input_DisplayDiscardPile(PlayerCombatObject p)
		{
			ShowPanel(cardViewerMenu);
			cardViewerMenu.DisplayDiscardPile(p);
		}

		public void Input_ShowPanel(HUDPanel panel)
		{
			ShowPanel(panel);
		}

		public void Input_HidePanel()
		{
			HideTopPanel();
		}

		public void Input_ShowPauseMenu()
		{
			ShowPanel(pausePanel);
		}

		public override void HideTopPanel()
		{
			if (states.Count > 0)
			{
				HUDPanel panel = states.Pop();
				panel.gameObject.SetActive(false);
			}
			else
			{
				Input_ShowPauseMenu();
			}
		}
	}
}