using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.PauseMenu
{
	public class PauseMenu : HUDPanel
	{
		[Header("Prefabs")]
		[SerializeField]
		GameObject settingsMenuPrefab;

		[Header("References")]
		[SerializeField]
		Button settingsButton;
		[SerializeField]
		QuitMenu quitPanel;
		Button backToMainMenuButton;
		Button quitGameButton;

		SettingsMenu settingsPanel;

		protected override void Start()
		{
			base.Start();
			settingsPanel = GameObject.Instantiate(settingsMenuPrefab, transform).GetComponent<SettingsMenu>();
			settingsPanel.gameObject.name = settingsMenuPrefab.name;
			settingsPanel.gameObject.SetActive(false);
			settingsButton.onClick.AddListener(delegate { Input_ShowSettings(); });
			/*
			backToMainMenuButton.onClick.AddListener(delegate { GameManager.Instance.ReturnToMainMenu(); });
			quitGameButton.onClick.AddListener(delegate { GameManager.Instance.QuitGame(); });
			*/
		}

		public void Input_ReturnToLevel()
		{
			GameManager.Instance.CurrentSceneSettings.HideTopPanel();
		}

		public void Input_ShowSettings()
		{
			GameManager.Instance.CurrentSceneSettings.ShowPanel(settingsPanel);
		}

		public void Input_ShowQuit()
		{
			GameManager.Instance.CurrentSceneSettings.ShowPanel(quitPanel);
		}
	}
}