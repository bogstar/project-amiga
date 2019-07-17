using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amiga.Inventory
{
	[CreateAssetMenu(fileName = "New Item", menuName = "Characters/Items/Item")]
	public class ItemScriptableObject : ScriptableObject
	{
		public new string name;
		[TextArea()]
		public string description;
		public Sprite image;
		public Vector2Int dimensions;
	}
}