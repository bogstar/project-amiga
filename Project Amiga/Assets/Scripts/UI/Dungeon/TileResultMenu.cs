using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Dungeon
{
	public class TileResultMenu : HUDPanel
	{
		public Text description;
		public Button combatButton;
		public GameObject loadingContent;
		public GameObject selectionContent;
		public Button startCombatButton;
		public Text loadingPercentageText;

		public void Display(EncounterScriptableObject encounter, System.Action OnCombatStarted)
		{
			combatButton.interactable = false;
			combatButton.onClick.RemoveAllListeners();
			if (encounter != null)
			{
				description.text = "You found:\n" + encounter.enemies.Length.ToString() + " enemies!";
				combatButton.interactable = true;
				combatButton.onClick.AddListener(delegate { OnCombatStarted(); });
				combatButton.onClick.AddListener(delegate { OnClick(); });
			}
			else
			{
				description.text = "You found:\nNothing!";
			}
		}

		void Update()
		{
			if (loadingContent != null && loadingContent.activeSelf)
			{
				if (GameManager.Instance.levelLoadingOperation != null && GameManager.Instance.levelLoadingOperation.progress < .9f)
				{
					loadingPercentageText.text = "Loading " + (GameManager.Instance.levelLoadingOperation.progress * 100f).ToString("#");
				}
				else
				{
					loadingPercentageText.text = "Loading: 100%";
					startCombatButton.interactable = true;
				}
			}
		}

		void OnClick()
		{
			selectionContent.SetActive(false);
			loadingContent.SetActive(true);
			startCombatButton.onClick.AddListener(delegate { GameManager.Instance.TriggerNextLevel(); });
		}
	}
}