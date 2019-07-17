using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	public PlayerCombatObject player;
	public PlayerHand hand;
	public Text nameBar;
	public GameObject armorImage;
	public Text armorText;
	public Image healthBar;
	public Text healthTextual;
	public float healthBarWidth = 165;
	public RectTransform manaBar;
	public Text manaTextual;
	public float manaBarWidth = 35;
	public Color armoredColorHealthBar;
	public Color healthBarColor;
	public StatusEffectBar statusEffectBar;
	public Text drawPileCardNumber;
	public Text discardPileCardNumber;
	public Image skull;

	public void UpdateUI()
	{
		transform.localPosition = new Vector3(0, player.ModelInfo.modelHeight, 0);

		if (player.Block > 0)
		{
			armorImage.SetActive(true);
			healthBar.color = armoredColorHealthBar;
			armorText.text = player.Block.ToString();
		}
		else
		{
			armorImage.SetActive(false);
			healthBar.color = healthBarColor;
		}

		float healthPercentage = (player.Health / (float)player.MaxHealth) * healthBarWidth;
		healthBar.rectTransform.sizeDelta = new Vector2(healthPercentage, healthBar.rectTransform.sizeDelta.y);
		healthTextual.text = player.Health + "/" + player.MaxHealth;
		float manaPercentage = (player.Mana / (float)player.MaxMana) * manaBarWidth;
		manaPercentage = Mathf.Clamp(manaPercentage, 0, manaBarWidth);
		manaBar.sizeDelta = new Vector2(manaPercentage, manaBar.sizeDelta.y);
		manaTextual.text = player.Mana + "/" + player.MaxMana;
		nameBar.text = player.Entity.Name;
		skull.gameObject.SetActive(player.IsDead);

		drawPileCardNumber.text = player.hand.drawPile.Count.ToString();
		discardPileCardNumber.text = player.hand.discardPile.Count.ToString();

		statusEffectBar.UpdateUI((player.StatusEffects == null) ? null : player.StatusEffects.ToArray());
	}
}