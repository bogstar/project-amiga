using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : Singleton<SessionManager>
{
	public PlayerCharacterScriptableObject[] startingCharacters;
	public DungeonScriptableObject[] startingDungeons;

	public int Gold { get; private set; }
	public List<PlayerCharacterScriptableObject> unlockedCharacters = new List<PlayerCharacterScriptableObject>();
	public List<DungeonScriptableObject> unlockedDungeons = new List<DungeonScriptableObject>();

	public void StartSession(int gold)
	{
		Gold = gold;
		foreach (var c in startingCharacters)
		{
			unlockedCharacters.Add(c);
		}
		foreach (var d in startingDungeons)
		{
			unlockedDungeons.Add(d);
		}

		GameManager.Instance.SaveProgress();
	}

	public void EnterDungeon(DungeonScriptableObject dungeon)
	{
		GameManager.Instance.ChangeState(GameManager.State.Dungeon);
		GameManager.Instance.SpawnRunManager();
		RunManager.Instance.StartNewRun(dungeon, OnEndRun);
	}

	void OnEndRun()
	{
		GameManager.Instance.ChangeState(GameManager.State.Town);
	}

	public void ChangeGold(int gold)
	{
		Gold += gold;
	}

	public void AbortSession()
	{
		GameObject.Destroy(gameObject);
	}

	void OnApplicationQuit()
	{
		EndSession();
	}

	public void EndSession()
	{
		GameManager.Instance.SaveProgress();
		AbortSession();
	}
}