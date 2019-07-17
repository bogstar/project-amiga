using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	[Header("Debug")]
	public bool debugMode;
	public SceneLoader.Scene debugStartingScene;
	public DungeonScriptableObject debugDungeon;
	public EncounterScriptableObject debugCombatEncounter;

	[Header("------------------")]
	[SerializeField]
	GameObject sessionManagerPrefab;
	[SerializeField]
	GameObject runManagerPrefab;
	[SerializeField]
	GameObject audioManagerPrefab;

	[SerializeField]
	UnityEngine.Audio.AudioMixerGroup mixer;

	public EquipmentManager equipmentManager;

	public Amiga.UI.SceneSettings CurrentSceneSettings { get; private set; }

	public static string CurrentGameVersion
	{
		get
		{
			if(Resources.Load<TextAsset>("Version/version") == null)
			{
				return "0.0.0";
			}
			return Resources.Load<TextAsset>("Version/version").text;
		}
	}

	public AsyncOperation levelLoadingOperation;

	public State GameState { get; private set; }

	SceneLoader sceneLoader;

	protected override void Awake()
	{
		base.Awake();

		if (isDestroyed)
			return;

		sceneLoader = gameObject.AddComponent<SceneLoader>();
		ChangeState(State.MainMenu);

		if (debugMode == false)
		{
			if (SceneManager.GetActiveScene().name != "_MainMenu_")
			{
				GameObject.Destroy(SessionManager.Instance.gameObject);
				GameObject.Destroy(RunManager.Instance.gameObject);
				ChangeState(State.MainMenu);
			}
		}
		else
		{
			switch (debugStartingScene)
			{
				case SceneLoader.Scene.Town:
					StartNewGame();
					break;
				case SceneLoader.Scene.Dungeon:
					StartNewGame();
					SessionManager.Instance.EnterDungeon(debugDungeon);
					break;
				case SceneLoader.Scene.Combat:
					StartNewGame();
					SessionManager.Instance.EnterDungeon(debugDungeon);
					RunManager.Instance.StartCombat(debugCombatEncounter);
					break;
			}
		}
	}

	void Start()
	{
		Instantiate(audioManagerPrefab).name = audioManagerPrefab.name;

		float calc = GetVolumeLevel() / 100f * 55f - 55f;
		mixer.audioMixer.SetFloat("Volume", calc);	

		// Debug
		//StartCoroutine(DebugLog());
	}

	public int GetVolumeLevel()
	{
		return PlayerPrefs.GetInt("options_volume", 80);
	}

	public void SetResolution(Resolution newRes, bool fullscreen)
	{
		PlayerPrefs.SetInt("Screenmanager Resolution Width", newRes.width);
		PlayerPrefs.SetInt("Screenmanager Resolution Height", newRes.height);
		PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", fullscreen ? 1 : 0);

		Screen.SetResolution(newRes.width, newRes.height, fullscreen);
	}

	public void SetVolumeLevel(float level, int percentage)
	{
		mixer.audioMixer.SetFloat("Volume", level);

		PlayerPrefs.SetInt("options_volume", percentage);
	}

	public void SpawnRunManager()
	{
		Instantiate(GameManager.Instance.runManagerPrefab).name = runManagerPrefab.name;
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void StartNewGame()
	{
		StartGame(0);
	}

	public void ResumeGame()
	{
		StartGame(PlayerPrefs.GetInt("gold", 0));
	}

	void StartGame(int gold)
	{
		ChangeState(State.Town);
		GameObject.Instantiate(sessionManagerPrefab).name = sessionManagerPrefab.name;
		SessionManager.Instance.StartSession(gold);
	}

	public void ReturnToMainMenu()
	{
		// End session
		SessionManager.Instance.EndSession();
		ChangeState(State.MainMenu);
	}

	public void TriggerNextLevel()
	{
		if(levelLoadingOperation != null)
		{
			if(levelLoadingOperation.progress >= .9f)
			{
				levelLoadingOperation.allowSceneActivation = true;
				levelLoadingOperation = null;
#if UNITY_EDITOR
				if (UnityEditor.Lightmapping.giWorkflowMode == UnityEditor.Lightmapping.GIWorkflowMode.Iterative)
				{
					DynamicGI.UpdateEnvironment();
				}
#endif
			}
		}
	}

	public void ChangeState(State newState, System.Action OnStateChangedCallback = null)
	{
		SceneLoader.Scene newLevelToLoad = SceneLoader.Scene.MainMenu;

		System.Action callbacks = OnSceneLoaded;
		if (OnStateChangedCallback != null)
		{
			callbacks += OnStateChangedCallback;
		}

		switch (newState)
		{
			case State.Town:
				newLevelToLoad = SceneLoader.Scene.Town;
				sceneLoader.LoadScene(newLevelToLoad, callbacks);
				break;
			case State.Dungeon:
				newLevelToLoad = SceneLoader.Scene.Dungeon;
				sceneLoader.LoadScene(newLevelToLoad, callbacks);
				break;
			case State.Combat:
				newLevelToLoad = SceneLoader.Scene.Combat;
				sceneLoader.LoadScene(newLevelToLoad, callbacks);
				break;
			default:
				sceneLoader.LoadScene(newLevelToLoad, callbacks);
				break;
		}
		GameState = newState;
	}

	void OnSceneLoaded()
	{
#if UNITY_EDITOR
		if (UnityEditor.Lightmapping.giWorkflowMode == UnityEditor.Lightmapping.GIWorkflowMode.Iterative)
		{
			DynamicGI.UpdateEnvironment();
		}
#endif
		CurrentSceneSettings = FindObjectOfType<Amiga.UI.SceneSettings>();
		if (RunManager.Instance != null && RunManager.Instance.OnGoldChange != null)
		{
			RunManager.Instance.OnGoldChange();
		}
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}

	public void SaveProgress()
	{
		if(GameState == State.Town)
		{
			PlayerPrefs.SetInt("run", 1);
			PlayerPrefs.SetInt("gold", SessionManager.Instance.Gold);
		}
	}

	public enum State
	{
		MainMenu,
		Town,
		Dungeon,
		Combat
	}
}