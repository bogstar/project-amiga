using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
	/// <summary>
	/// Returns position of the mouse in world space on the y provided.
	/// </summary>
	/// <param name="point">Screenpoint</param>
	/// <param name="camera">Reference camera</param>
	/// <param name="y">Y of the plane on which the position is projected</param>
	/// <returns></returns>
	public static Vector3 GetCursorInWorldPosition(Vector3 point, Camera camera, float y)
	{
		Ray ray = camera.ScreenPointToRay(point);
		float distance = ray.origin.y / ray.direction.y;

		return (ray.origin + ray.direction * -distance) + Vector3.up * y;
	}

	/// <summary>
	/// Returns combined string with values.
	/// </summary>
	/// <param name="text">String with meta-values</param>
	/// <param name="values">Values to insert</param>
	/// <param name="symbol">Symbol on which to split</param>
	/// <returns></returns>
	public static string GetString(string text, List<string> values, char symbol)
	{
		string[] splits = text.Split(symbol);
		string finalString = "";

		int i = -1;
		int j = 0;
		foreach (var split in splits)
		{
			i++;
			if (i % 2 == 0)
			{
				finalString += split;
				continue;
			}

			finalString += values[j];

			j++;
		}

		return finalString;
	}

	/// <summary>
	/// Copies component
	/// </summary>
	/// <param name="original"></param>
	/// <param name="destination"></param>
	/// <returns></returns>
	public static Component CopyComponent(Component original, GameObject destination)
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}
}

public enum Layer
{
	Clickable = 8
}