using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
	public EnemyCombatObject enemy;

	public Text nameText;
	public Image healthBar;
	public Text healthTextual;
	public float healthBarWidth = 70;
	public Color armoredColorHealthBar;
	public Color healthBarColor;
	public GameObject armorImage;
	public Text armorText;
	public Image nextAbilityImage;
	public Text nextAbilityDamage;
	public StatusEffectBar statusEffectBar;
	public Image skull;

	public void UpdateUI()
	{
		transform.localPosition = new Vector3(0, enemy.ModelInfo.modelHeight, 0);

		if (enemy.Block > 0)
		{
			armorImage.SetActive(true);
			healthBar.color = armoredColorHealthBar;
			armorText.text = enemy.Block.ToString();
		}
		else
		{
			armorImage.SetActive(false);
			healthBar.color = healthBarColor;
		}

		float healthPercentage = (enemy.Health / (float)enemy.MaxHealth) * healthBarWidth;
		healthBar.rectTransform.sizeDelta = new Vector2(healthPercentage, healthBar.rectTransform.sizeDelta.y);
		healthTextual.text = enemy.Health + "/" + enemy.MaxHealth;
		nameText.text = enemy.Entity.Name;
		nextAbilityDamage.text = "";
		nextAbilityImage.enabled = true;
		skull.gameObject.SetActive(enemy.IsDead);

		if (enemy.NextAbility != null)
		{
			switch (enemy.NextAbility.type)
			{
				case EnemyAbilityScriptableObject.Type.Offensive:
					nextAbilityImage.sprite = FindObjectOfType<CombatManager>().enemyManager.damagingAbilitySprite;
					nextAbilityDamage.text = ((AbilityStep_Damage)enemy.NextAbility.abilityStepsWithTargetingData[0].abilityStep).amount.ToString();
					break;
				case EnemyAbilityScriptableObject.Type.Healing:
					nextAbilityImage.sprite = FindObjectOfType<CombatManager>().enemyManager.healingAbilitySprite;
					nextAbilityDamage.text = ((AbilityStep_Restore)enemy.NextAbility.abilityStepsWithTargetingData[0].abilityStep).amount.ToString();
					break;
				case EnemyAbilityScriptableObject.Type.Buffing:
					nextAbilityImage.sprite = FindObjectOfType<CombatManager>().enemyManager.buffingAbilitySprite;
					break;
				case EnemyAbilityScriptableObject.Type.Blocking:
					nextAbilityImage.sprite = FindObjectOfType<CombatManager>().enemyManager.blockingAbilitySprite;
					break;
				case EnemyAbilityScriptableObject.Type.Debuffing:
					nextAbilityImage.sprite = FindObjectOfType<CombatManager>().enemyManager.debuffingAbilitySprite;
					break;
				default:
					nextAbilityImage.enabled = false;
					break;
			}
		}
		else
		{
			nextAbilityImage.enabled = false;
		}

		statusEffectBar.UpdateUI((enemy.StatusEffects == null) ? null : enemy.StatusEffects.ToArray());
	}
}