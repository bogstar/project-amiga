using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.MainMenu
{
	public class QuitMenu : HUDPanel
	{
		[Header("References")]
		[SerializeField]
		Button confirmButton;

		protected override void Start()
		{
			base.Start();
			confirmButton.onClick.AddListener(delegate { GameManager.Instance.QuitGame(); });
			Input_HideModal();
		}

		public void Input_HideModal()
		{
			gameObject.SetActive(false);
		}
	}
}