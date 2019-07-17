using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
	static char m_directorySeparatorChar = '/';
	public static char DirectorySeparatorChar { get { return m_directorySeparatorChar; } }

	static string m_projectPath = Application.dataPath.Replace(DirectorySeparatorChar + "Assets", "");
	public static string ProjectPath { get { return m_projectPath; } }

	static string m_versionPathFolder = DirectorySeparatorChar + "Version";
	public static string VersionPathFolder { get { return ProjectPath + m_versionPathFolder; } }

	static string m_versionsPathFolder = DirectorySeparatorChar + "Versions";
	public static string VersionsPathFolder { get { return VersionPathFolder + m_versionsPathFolder; } }

	static string m_versionPath = m_directorySeparatorChar + "version.txt";
	public static string VersionPath { get { return VersionPathFolder + m_versionPath; } }

	static string m_databasePath = "Database";
	public static string DatabasePath { get { return m_databasePath; } }

	static string m_currentVersionFolderPath = DirectorySeparatorChar + "Version";
	public static string CurrentVersionFolderPath { get { return ResourcesPath + m_currentVersionFolderPath; } }

	static string m_currentVersionPath = DirectorySeparatorChar + "version.txt";
	public static string CurrentVersionPath { get { return CurrentVersionFolderPath + m_currentVersionPath; } }

	static string m_cardsPath = m_databasePath + m_directorySeparatorChar + "Cards";
	public static string CardsPath { get { return m_cardsPath; } }

	static string m_resourcesPath =  m_directorySeparatorChar + "Resources";
	public static string ResourcesPath { get { return Application.dataPath + m_resourcesPath; } }

	static string m_charactersPath = m_databasePath + m_directorySeparatorChar + "Characters";
	public static string CharactersPath { get { return m_charactersPath; } }

	static string m_dungeonsPath = m_databasePath + m_directorySeparatorChar + "Dungeons";
	public static string DungeonsPath { get { return m_dungeonsPath; } }

	static string m_equipmentPath = m_charactersPath + m_directorySeparatorChar + "Equipment";
	public static string EquipmentPath { get { return m_equipmentPath; } }

	static string m_buildPath = m_directorySeparatorChar + "Build";
	public static string BuildPath { get { return ProjectPath + m_buildPath; } }

	static char m_separatorChar = '$';
	public static char SeparatorChar { get { return m_separatorChar; } }

	static string m_gameVersion = "";
	public static string GameVersion
	{
		get
		{
			if (m_gameVersion == "")
				GenerateVersions();

			return m_gameVersion;
		}
	}

	static string[] m_previousVersions = new string[0];
	public static string[] PreviousVesions { get { return m_previousVersions; } }


	static void GenerateVersions()
	{
		if (!Directory.Exists(CurrentVersionFolderPath))
		{
			Directory.CreateDirectory(CurrentVersionFolderPath);
		}
		if (!Directory.Exists(VersionPathFolder))
		{
			Directory.CreateDirectory(VersionPathFolder);
		}
		if (!File.Exists(VersionPath))
		{
			AddVersion("0.0.0", "Empty patch notes");
		}
		else
		{
			string file = File.ReadAllText(VersionPath);

			if (file.Contains(SeparatorChar.ToString()))
			{
				string[] versions = file.Split(SeparatorChar);

				m_previousVersions = versions;

				File.WriteAllText(CurrentVersionPath, versions[versions.Length - 1]);

				m_gameVersion = versions[versions.Length - 1];
			}
			else
			{
				m_gameVersion = file;
			}
		}
	}

	public static void AddVersion(string version, string patchNotes)
	{
		if (!Directory.Exists(VersionsPathFolder))
		{
			Directory.CreateDirectory(VersionsPathFolder);
		}

		File.AppendAllText(VersionsPathFolder + DirectorySeparatorChar + version + ".txt", patchNotes);
		File.AppendAllText(VersionPath, SeparatorChar + version);
	}

	public static void ReloadVersionsFromFile()
	{
		m_gameVersion = "";
		m_previousVersions = new string[0];
		GenerateVersions();
	}

	public static void DeleteAllVersions()
	{
		m_gameVersion = "";
		m_previousVersions = new string[0];
		Directory.Delete(VersionPathFolder, true);
		Directory.Delete(BuildPath, true);
		Directory.Delete(CurrentVersionFolderPath, true);
	}

	public static bool VersionExists(string version)
	{
		foreach (string s in m_previousVersions)
		{
			if (version == s)
				return true;
		}
		if (version == m_gameVersion)
		{
			return true;
		}
		return false;
	}
}