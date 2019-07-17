using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	bool loadingScene;
	bool activateImmediately;
	bool canActivateScene;

	AsyncOperation levelLoadingOperation;

	System.Action OnLoadCallback;

	/// <summary>
	/// Returns a float from 0 to 1 when loading scene, and -1 when not loading.
	/// </summary>
	/// <returns></returns>
	public float GetProgress()
	{
		if (loadingScene)
		{
			return Mathf.Min(levelLoadingOperation.progress / .9f, 1f);
		}
		else
		{
			return -1;
		}
	}

	public void ActivateScene()
	{
		canActivateScene = true;
	}

	public void LoadScene(Scene scene, System.Action OnSceneLoadedCallback = null, bool disallowActivation = false)
	{
		int sceneId = (int)scene;
		if(OnSceneLoadedCallback != null)
		{
			OnLoadCallback += OnSceneLoadedCallback;
		}

		activateImmediately = !disallowActivation;
		canActivateScene = false;

		StartCoroutine( LoadSceneAsync(sceneId) );
	}

	IEnumerator LoadSceneAsync(int sceneId)
	{
		loadingScene = true;

		levelLoadingOperation = SceneManager.LoadSceneAsync(sceneId);

		if (activateImmediately == true)
		{
			levelLoadingOperation.allowSceneActivation = true;

			while (levelLoadingOperation.progress < 1f)
			{
				yield return null;
			}
		}
		else
		{
			levelLoadingOperation.allowSceneActivation = false;

			while (levelLoadingOperation.progress < .9f || !canActivateScene)
			{
				yield return null;
			}

			levelLoadingOperation.allowSceneActivation = true;

			while (levelLoadingOperation.progress < 1f)
			{
				yield return null;
			}
		}

		yield return null;

		if (OnLoadCallback != null)
		{
			OnLoadCallback();
			OnLoadCallback = null;
		}
		
		loadingScene = false;

		yield return null;
	}

	public enum Scene
	{
		MainMenu = 0,
		Town = 1,
		Dungeon = 2,
		Combat = 3
	}
}