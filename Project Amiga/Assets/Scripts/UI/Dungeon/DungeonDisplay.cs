using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Dungeon
{
	public class DungeonDisplay : SceneSettings
	{
		[Header("Prefabs")]
		[SerializeField]
		GameObject tilePrefab;

		[Header("References")]
		[SerializeField]
		GameObject dungeonHolder;
		[SerializeField]
		GameObject tilesHolder;
		[SerializeField]
		TileResultMenu tileResultModal;
		[SerializeField]
		Text dungeonTitleText;
		[SerializeField]
		Text goldAmountText;
		[SerializeField]
		Amiga.UI.PauseMenu.PauseMenu pausePanel;
		public InventoryMenu inventoryPanel;

		EncounterScriptableObject currentEncounterSelected;

		void Start()
		{
			GameObject.Instantiate(versionPrefab, dungeonHolder.transform);
			if (RunManager.Instance != null && RunManager.Instance.OnGoldChange == null)
			{
				RunManager.Instance.OnGoldChange += OnGoldChange;
			}

			OnGoldChange();

			pausePanel.gameObject.SetActive(false);

			dungeonTitleText.text = RunManager.Instance.CurrentDungeon.name;

			foreach (var dt in RunManager.Instance.Tiles)
			{
				GameObject go = GameObject.Instantiate(tilePrefab, tilesHolder.transform);
				DungeonTileGameObject newTileGO = go.GetComponent<DungeonTileGameObject>();
				newTileGO.Init(dt, Input_TileSelected);
			}
		}

		void Refresh()
		{
			foreach (Transform tile in tilesHolder.transform)
			{
				DungeonTileGameObject dt = tile.gameObject.GetComponent<DungeonTileGameObject>();
				dt.Refresh();
			}
		}

		void OnGoldChange()
		{
			goldAmountText.text = RunManager.Instance.Gold.ToString() + " gold";
		}

		public void Input_TileSelected(DungeonTile tile)
		{
			if (!tile.Visited)
			{
				currentEncounterSelected = tile.Encounter;
				ShowPanel(tileResultModal);
				tileResultModal.Display(tile.Encounter, StartCombat);
			}
			RunManager.Instance.TileVisited(tile);
			Refresh();
		}

		void StartCombat()
		{
			RunManager.Instance.OnGoldChange = null;
			RunManager.Instance.StartCombat(currentEncounterSelected);
		}

		public void Input_DisplayIventory()
		{
			ShowPanel(inventoryPanel);
		}

		public void Input_ShowPanel(HUDPanel panel)
		{
			ShowPanel(panel);
		}

		public void Input_EndRun()
		{
			RunManager.Instance.EndRun();
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
				if (states.Peek() is InventoryMenu)
				{
					//((InventoryMenu)states.Peek()).ReleaseItem();
				}
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