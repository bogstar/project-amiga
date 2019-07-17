using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amiga.Inventory;

namespace Amiga.UI.Inventory
{
	public class InventorySlot : UISlot
	{
		public Vector2Int Position { get; private set; }
		public InventorySlot MainSlot { get; private set; }
		public bool IsInfiniteStorage()
		{
			return false;
		}

		float slotDimensions;


		public void Init(Vector2Int position, float slotDimensions)
		{
			gameObject.name = position.x + " " + position.y;
			this.Position = position;
			this.slotDimensions = slotDimensions;
		}

		public override void SetItem(Item item)
		{
			this.Item = item;

			if (item == null)
			{
				
				ItemImage.gameObject.SetActive(false);
				MainSlot = null;
			}
			else
			{
				MainSlot = this;
				ItemImage.gameObject.SetActive(true);
				if(item.Dimensions.x == item.Dimensions.y)
				{
					ItemImage.preserveAspect = true;
				}
				else
				{
					ItemImage.preserveAspect = false;
				}
				ItemImage.sprite = item.Image;
				ItemImage.rectTransform.sizeDelta = new Vector2(item.Dimensions.y * slotDimensions, item.Dimensions.x * slotDimensions);
				ItemImage.transform.localScale = Vector3.one;
				ItemImage.rectTransform.anchoredPosition = new Vector2(Position.y * slotDimensions + slotDimensions / 2, -Position.x * slotDimensions - slotDimensions / 2);
				ItemImage.rectTransform.anchoredPosition += new Vector2((item.Dimensions.y - 1) * slotDimensions / 2, -(item.Dimensions.x - 1) * slotDimensions / 2);
			}
		}
		
		public void SetPartItem(Item item, InventorySlot mainSlot)
		{
			this.MainSlot = mainSlot;
			this.Item = item;
			ItemImage.gameObject.SetActive(false);
		}
	}
}