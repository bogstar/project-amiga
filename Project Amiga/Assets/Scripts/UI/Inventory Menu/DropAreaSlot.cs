using System.Collections;
using System.Collections.Generic;
using Amiga.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Inventory
{
	public class DropAreaSlot : UISlot
	{
		public override void SetItem(Item item)
		{
			Item = item;

			if (item == null)
			{
				ItemImage.gameObject.SetActive(false);
			}
			else
			{
				ItemImage.gameObject.SetActive(true);
				ItemImage.sprite = item.Image;
			}
		}
	}
}