using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Town
{
	public class CollectionMenu : HUDPanel
	{
		[Header("References")]
		[SerializeField]
		Text goldText;

		void OnEnable()
		{
			goldText.text = SessionManager.Instance.Gold.ToString() + " gold";
		}
	}
}