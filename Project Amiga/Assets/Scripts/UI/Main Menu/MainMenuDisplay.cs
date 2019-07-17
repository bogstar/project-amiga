using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI
{
	public class MainMenuDisplay : SceneSettings
	{
		[Header("Prefabs")]
		[SerializeField]
		GameObject settingsMenuPrefab;

		[Header("References")]
		[SerializeField]
		GameObject mainMenuHolder;
		[SerializeField]
		Amiga.UI.MainMenu.CreditsMenu creditsPanel;
		[SerializeField]
		Amiga.UI.MainMenu.QuitMenu quitPanel;
		[SerializeField]
		Amiga.UI.MainMenu.LevelLoadingMenu levelLoadingPanel;
		[SerializeField]
		Button settingsButton;
		[SerializeField]
		Button continueButton;
		[SerializeField]
		Text loadingProgress;

		SettingsMenu settingsPanel;

		void Start()
		{
			GameObject.Instantiate(versionPrefab, mainMenuHolder.transform);
			settingsPanel = GameObject.Instantiate(settingsMenuPrefab, mainMenuHolder.transform).GetComponent<SettingsMenu>();
			settingsPanel.gameObject.name = settingsMenuPrefab.name;
			levelLoadingPanel.gameObject.SetActive(false);
			creditsPanel.gameObject.SetActive(false);
			quitPanel.gameObject.SetActive(false);
			settingsPanel.gameObject.SetActive(false);
			continueButton.interactable = (PlayerPrefs.GetInt("run", 0) == 1) ? true : false;
			settingsButton.onClick.AddListener(delegate { Input_ShowSettings(); });
		}

		public void Input_ShowLevelLoading()
		{
			//ShowPanel(settingsPanel);
			Input_StartNew();
		}

		public void Input_ShowSettings()
		{
			ShowPanel(settingsPanel);
		}

		public void Input_ShowCredits()
		{
			ShowPanel(creditsPanel);
		}

		public void Input_ShowQuitConfirm()
		{
			ShowPanel(quitPanel);
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
				Input_ShowQuitConfirm();
			}
		}

		public void Input_StartNew()
		{
			GameManager.Instance.StartNewGame();
		}

		public void Input_Continue()
		{
			GameManager.Instance.ResumeGame();
		}
	}
}