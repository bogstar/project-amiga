#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

public class BuildPipelineEditorWindow : EditorWindow
{
	int selectedIndex;
	string currentVersion;
	string newVersion;
	string patchNotes;
	Vector2 scroll;

	[MenuItem("My Code/Make Version")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<BuildPipelineEditorWindow>("Build Pipeline");
	}

	private void OnGUI()
	{
		GUILayout.Label("Current Version: " + currentVersion);
		GUILayout.Label("New Version: ");

		newVersion = EditorGUILayout.TextField(newVersion);

		int numberOfLines = 10;

		patchNotes = EditorGUILayout.TextArea(patchNotes, GUILayout.Height(15 * numberOfLines));

		string[] strings = { "Windows 32-bit" };

		GUI.enabled = true;
		selectedIndex = EditorGUILayout.Popup(selectedIndex, strings);

		if (FileManager.VersionExists(newVersion))
		{
			GUILayout.Label("This version already exists.");
			GUI.enabled = false;
		}
		else
		{
			GUI.enabled = true;
		}
		if (GUILayout.Button("Save"))
		{
			if (!Directory.Exists(FileManager.ResourcesPath + FileManager.DirectorySeparatorChar + "Version"))
			{
				Directory.CreateDirectory(FileManager.ResourcesPath + FileManager.DirectorySeparatorChar + "Version");
			}

			File.WriteAllText(FileManager.ResourcesPath + FileManager.DirectorySeparatorChar + "Version" + FileManager.DirectorySeparatorChar + "version.txt", newVersion);
		}
		if (GUILayout.Button("Build"))
		{
			BuildManager.BuildGame(newVersion, selectedIndex, patchNotes);
			this.Close();
		}
		GUI.enabled = true;
	}

	void BuildGame()
	{
		BuildManager.BuildGame(newVersion, selectedIndex, patchNotes);
	}

	void RefreshVersion()
	{
		currentVersion = GameManager.CurrentGameVersion;
		if (currentVersion == "0.0.0")
			newVersion = "0.0.1";
		else
			newVersion = currentVersion + "a";
	}

	void OnEnable()
	{
		RefreshVersion();
	}
}
#endif