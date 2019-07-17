using Amiga.UI.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCard : MonoBehaviour, ITooltip
{
	public Text nameText;
	public Text descText;
	public Text manaCostText;
	public Text typeText;
	public Image image;
	public UISlot.TooltipData tooltip;

	public void Display(CardGameObject card)
	{
		if (card != null && card.ethereal)
		{
			nameText.text = "ETHEREAL " + card.card.name;
		}
		else
		{
			nameText.text = card.card.name;
		}

		Dictionary<string, int> descriptionTags = new Dictionary<string, int>();
		var descriptionParts = card.card.description.Split(FileManager.SeparatorChar);
		if (card.card.description[0] == FileManager.SeparatorChar)
		{
			for (int i = 0; i < descriptionParts.Length; i += 2)
			{
				descriptionTags.Add(descriptionParts[i], i);
			}
		}
		else
		{
			for (int i = 1; i < descriptionParts.Length; i+=2)
			{
				descriptionTags.Add(descriptionParts[i], i);
			}
		}

		foreach (var step in card.card.stepValue)
		{
			if (descriptionTags.ContainsKey(step.descriptionTag.ToString()))
			{
				var actualValue = card.card.abilityStepsWithTargetingData[step.stepIndex].abilityStep.GetValueForIndex(step.valueIndex);

				descriptionParts[descriptionTags[step.descriptionTag.ToString()]] = actualValue;
			}
		}

		descText.text = "";
		foreach (var text in descriptionParts)
		{
			descText.text += text;
		}

		//descText.text = Utility.GetString(card.card.description, card.card., FileManager.SeparatorChar);
		manaCostText.text = card.card.manaCost.ToString();
		typeText.text = card.card.data.type.ToString();
		image.sprite = card.card.image;
		tooltip = GenerateTooltip(card);
	}

	public void Display(Card card)
	{
		nameText.text = card.name;
		//descText.text = Utility.GetString(card.description, card.values, FileManager.SeparatorChar);
		manaCostText.text = card.manaCost.ToString();
		typeText.text = card.data.type.ToString();
		image.sprite = card.image;
	}

	UISlot.TooltipData GenerateTooltip(CardGameObject card)
	{
		UISlot.TooltipData tooltip = new UISlot.TooltipData();

		if (card.card.tooltip.Trim() == "")
		{
			tooltip.show = false;
			return tooltip;
		}

		tooltip.title = card.card.name;
		tooltip.content = card.card.tooltip;
		tooltip.show = true;

		return tooltip;
	}

	public UISlot.TooltipData GetTooltip()
	{
		return tooltip;
	}
}