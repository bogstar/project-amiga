using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.PauseMenu
{
	public class QuitMenu : HUDPanel
	{
		[Header("References")]
		[SerializeField]
		Button mainMenuButton;
		[SerializeField]
		Button quitGameButton;

		protected override void Start()
		{
			base.Start();
			quitGameButton.onClick.AddListener(delegate { GameManager.Instance.QuitGame(); });
			mainMenuButton.onClick.AddListener(delegate { GameManager.Instance.ReturnToMainMenu(); });
			gameObject.SetActive(false);
		}

		public void Input_HideModal()
		{
			GameManager.Instance.CurrentSceneSettings.HideTopPanel();
		}
	}
}