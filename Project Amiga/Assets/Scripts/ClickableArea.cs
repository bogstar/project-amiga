using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableArea : MonoBehaviour
{
	/// <summary>
	/// Flag as selected.
	/// </summary>
	public bool Selected { get; set; }

	new BoxCollider collider;
	new MeshRenderer renderer;

	Color selectableColor = Color.green;
	Color selectedColor = Color.cyan;
	Color mouseOverColor = Color.yellow;


	void Awake()
	{
		collider = GetComponent<BoxCollider>();
		renderer = GetComponent<MeshRenderer>();
	}

	/// <summary>
	/// Display or hide the Clickable area.
	/// </summary>
	/// <param name="show"></param>
	public void ShowArea(bool show)
	{
		collider.enabled = show;
		renderer.enabled = show;
		renderer.material.color = Selected ? selectedColor : selectableColor;
	}

	/// <summary>
	/// Highlight the Clickable area.
	/// </summary>
	/// <param name="mouse"></param>
	public void MouseOver(bool mouse)
	{
		renderer.material.color = mouse ? mouseOverColor : (Selected ? selectedColor : selectableColor);
	}

	/// <summary>
	/// Refreshes the Clickable area.
	/// </summary>
	public void Refresh()
	{
		Selected = false;
		MouseOver(false);
	}
}