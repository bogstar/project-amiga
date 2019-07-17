using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amiga.Inventory;

namespace Amiga.UI.Inventory
{
	public class UISlot : MonoBehaviour, ITooltip
	{
		[Header("References - UISlot")]
		public Image ItemImage;

		public Item Item { get; protected set; }
		public Image SlotImage { get; private set; }

		protected Color startingColor = new Color(1, 1, 1, 100 / 255f);
		protected Color highlightColor = Color.green;


		public void SetImages(Image itemImage)
		{
			ItemImage = itemImage;
		}

		public virtual void Highlight(bool highlight)
		{
			SlotImage.color = highlight ? highlightColor : startingColor;
		}

		public virtual void GrabItem(bool grab)
		{
			if (grab)
				ItemImage.color = new Color(ItemImage.color.r, ItemImage.color.g, ItemImage.color.b, .5f);
			else
				ItemImage.color = new Color(ItemImage.color.r, ItemImage.color.g, ItemImage.color.b);
		}

		public virtual void SetItem(Item item)
		{
			this.Item = item;

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

		public TooltipData GetTooltip()
		{
			TooltipData tooltip = new TooltipData();

			if(Item != null)
			{
				tooltip.show = true;
				tooltip.title = Item.Name;
				tooltip.content = Item.Description;
				if (Item is Equipment equipment)
				{
					tooltip.content += "\n\nSlot: " + equipment.slot.ToString() + "\nCan wear: ";
					for (int i = 0; i < equipment.Wearers.Length; i++)
					{
						tooltip.content += equipment.Wearers[i].name;
						if (equipment.Wearers.Length > 1 && i != equipment.Wearers.Length - 1)
						{
							tooltip.content += ", ";
						}
					}
				}
				tooltip.image = Item.Image;
			}
			else
			{
				tooltip.show = false;
			}

			return tooltip;
		}

		public UISlot GetSlot()
		{
			return this;
		}

		public struct TooltipData
		{
			public bool show;
			public string title;
			public string content;
			public Sprite image;
		}
	}

	public interface ITooltip
	{
		UISlot.TooltipData GetTooltip();
	}

	public interface IDroppable
	{
		bool DropHere(Item item, UISlot targetSlot);
	}

	public interface IRemovable
	{
		void RemoveFromHere(UISlot slot);
	}
}