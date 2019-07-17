#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class BuildManager
{
	public static void BuildGame(string newVersion, int selectedIndex, string patchNotes)
	{
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.locationPathName = FileManager.BuildPath;
		options.locationPathName += FileManager.DirectorySeparatorChar + newVersion;
		if (selectedIndex == 0)
		{
			options.target = BuildTarget.StandaloneWindows;
			options.locationPathName += "/Windows32";
		}
		options.locationPathName += "/build_v-" + newVersion + ".exe";

		FileManager.AddVersion(newVersion, patchNotes);

		FileManager.ReloadVersionsFromFile();

		BuildPipeline.BuildPlayer(options);
	}

	[MenuItem("My Code/Reload Versions From File")]
	public static void Refresh()
	{
		FileManager.ReloadVersionsFromFile();
	}

	[MenuItem("My Code/Delete All Versions")]
	public static void DeleteAll()
	{
		FileManager.DeleteAllVersions();
	}
}
#endif