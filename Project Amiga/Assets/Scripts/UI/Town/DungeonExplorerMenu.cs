using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Town
{
	public class DungeonExplorerMenu : HUDPanel
	{
		[Header("Prefabs")]
		[SerializeField]
		GameObject dungeonPrefab;

		[Header("References")]
		[SerializeField]
		GameObject dungeonHolder;

		protected override void Start()
		{
			base.Start();
			foreach (var dungeon in SessionManager.Instance.unlockedDungeons)
			{
				GameObject newButton = GameObject.Instantiate(dungeonPrefab, dungeonHolder.transform);
				newButton.name = dungeonPrefab.name;
				newButton.transform.GetChild(0).GetComponent<Text>().text = dungeon.name;
				newButton.GetComponent<Button>().onClick.AddListener(delegate { SessionManager.Instance.EnterDungeon(dungeon); });
			}
		}
	}
}