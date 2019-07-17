using System.Collections;
using System.Collections.Generic;
using Amiga.UI.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Combat
{
	public class NativePowerItem : MonoBehaviour, Amiga.UI.Inventory.ITooltip
	{
		[Header("References")]
		[SerializeField]
		Text cooldownText;
		[SerializeField]
		Text manaText;
		[SerializeField]
		Text nameText;
		[SerializeField]
		Button button;

		public Button Button { get { return button; } }

		NativePowerScriptableObject power;

		public void Display(NativePowerScriptableObject power)
		{
			this.power = power;
			cooldownText.text = "";
			manaText.text = power.manaCost.ToString();
			nameText.text = power.name;

		}

		public UISlot.TooltipData GetTooltip()
		{
			UISlot.TooltipData tooltip = new UISlot.TooltipData();
			tooltip.image = power.picture;
			tooltip.title = power.name;
			tooltip.content = power.description;
			tooltip.content += "\n\nCost: " + power.manaCost;
			if (power.cooldown == 0)
			{
				tooltip.content += "\n No cooldown.";
			}
			else
			{
				tooltip.content += "\nCooldown: ";
				tooltip.content += power.cooldown.ToString() + " ";

				switch (power.cooldownType)
				{
					case NativePowerScriptableObject.CooldownType.Turn:
						tooltip.content += power.cooldown < 2 ? "turn." : "turns.";
						break;
					case NativePowerScriptableObject.CooldownType.Combat:
						tooltip.content += "per combat.";
						break;
					case NativePowerScriptableObject.CooldownType.Dungeon:
						tooltip.content += "per dungeon.";
						break;
				}
			}
			tooltip.show = true;

			return tooltip;
		}
	}
}