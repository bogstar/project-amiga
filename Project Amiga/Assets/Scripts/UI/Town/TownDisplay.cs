using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI
{
	public class TownDisplay : SceneSettings
	{
		[Header("References")]
		[SerializeField]
		GameObject townHolder;
		[SerializeField]
		Amiga.UI.PauseMenu.PauseMenu pausePanel;

		void Start()
		{
			GameObject.Instantiate(versionPrefab, townHolder.transform);
			pausePanel.gameObject.SetActive(false);
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