using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonTileGameObject : MonoBehaviour
{
	public DungeonTile Tile { get; private set; }

	public void Init(DungeonTile tile, System.Action<DungeonTile> OnClickCallback, bool startTile = false)
	{
		Tile = tile;
		GetComponent<Button>().onClick.AddListener(delegate { OnClickCallback(Tile); });
		Refresh();
	}

	public void Refresh()
	{
		gameObject.name = Tile.Room.Position.x.ToString() + " " + Tile.Room.Position.y.ToString();
		transform.localPosition = new Vector3(Tile.Room.Position.x * ((RectTransform)transform).sizeDelta.x, Tile.Room.Position.y * ((RectTransform)transform).sizeDelta.y, 0f);
		GetComponent<Image>().color = Color.white;
		gameObject.SetActive(false);

		if (RunManager.Instance.currentTile == Tile)
		{
			transform.GetChild(0).GetComponent<Text>().text = "(here)";
			GetComponent<Image>().color = Color.yellow;
		}
		else if (Tile.Visited)
		{
			transform.GetChild(0).GetComponent<Text>().text = "(visited)";
		}
		else
		{
			transform.GetChild(0).GetComponent<Text>().text = "?";
		}
		
		GetComponent<Button>().interactable = false;

		foreach (var n in RunManager.Instance.currentTile.Room.Neighbours)
		{
			if (Tile.Room == n)
			{
				GetComponent<Button>().interactable = true;
				Tile.TileSeen();
			}
		}

		if (Tile.Seen)
		{
			gameObject.SetActive(true);
		}
	}
}