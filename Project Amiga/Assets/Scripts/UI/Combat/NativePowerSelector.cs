using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amiga.UI.Combat
{
	public class NativePowerSelector : MonoBehaviour
	{
		[Header("Prefabs")]
		[SerializeField]
		GameObject powerItemPrefab;

		[Header("References")]
		[SerializeField]
		RectTransform powerMenuHolder;
		
		PlayerCombatObject player;


		void Start()
		{
			HideMenu();
		}

		public void Init(PlayerCombatObject player)
		{
			this.player = player;
		}

		public void ToggleMenu()
		{
			if (!powerMenuHolder.gameObject.activeSelf)
			{
				ShowMenu();
			}
			else
			{
				HideMenu();
			}
		}

		public void ShowMenu()
		{
			powerMenuHolder.gameObject.SetActive(true);
			PopulateMenu();
		}

		public void HideMenu()
		{
			powerMenuHolder.gameObject.SetActive(false);
		}

		void PopulateMenu()
		{
			foreach (Transform t in powerMenuHolder)
			{
				Destroy(t.gameObject);
			}

			foreach (var item in ((Player)player.Entity).nativePowers)
			{
				NativePowerItem newItem = Instantiate(powerItemPrefab, powerMenuHolder).GetComponent<NativePowerItem>();
				newItem.Button.onClick.AddListener(delegate { FindObjectOfType<CombatManager>().SelectNativePower(item, player); });
				newItem.Button.onClick.AddListener(delegate { HideMenu(); });
				newItem.Display(item);
			}
		}
	}
}
