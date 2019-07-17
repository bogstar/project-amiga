using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectBar : MonoBehaviour
{
	public GameObject prefab;

	public void UpdateUI(StatusEffect[] statusEffects)
	{
		foreach (Transform icon in transform)
		{
			GameObject.Destroy(icon.gameObject);
		}
		foreach (var effect in statusEffects)
		{
			GameObject newGO = GameObject.Instantiate(prefab, this.transform);
			newGO.GetComponent<Image>().sprite = effect.data.icon;
			newGO.transform.GetChild(0).GetComponent<Text>().text = effect.durationRemaining.ToString();
		}
	}
}