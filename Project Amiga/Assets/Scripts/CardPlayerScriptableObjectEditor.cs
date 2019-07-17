#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardPlayerScriptableObject))]
public class CardPlayerScriptableObjectEditor : Editor
{/*
	CardPlayerScriptableObject cardSO;
	List<int> values;

	private void OnEnable()
	{
		cardSO = (CardPlayerScriptableObject)target;
	}

	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();

		values = new List<int>();

		int i = 0;

		if(cardSO.abilityStepsWithTargetingData.Length < 1)
		{
			return;
		}
		foreach (var abilityStepWithTargetingData in cardSO.abilityStepsWithTargetingData)
		{
			if(abilityStepWithTargetingData.abilityStep is AbilityStep_Targetable)
			{
				AbilityStep_Targetable abilityStep = abilityStepWithTargetingData.abilityStep as AbilityStep_Targetable;

				values.Add(abilityStep.amount);

				GUILayout.Label(i + "-" + abilityStepWithTargetingData.abilityStep.name.ToString() + " amount: " + abilityStep.amount);
				i++;
			}
			if (abilityStepWithTargetingData.abilityStep is AbilityStep_AfflictStatusEffect)
			{
				AbilityStep_AfflictStatusEffect abilityStep = abilityStepWithTargetingData.abilityStep as AbilityStep_AfflictStatusEffect;

				if(abilityStep.statusEffect.behaviour is ThornsBehaviour)
				{
					values.Add(((ThornsBehaviour)abilityStep.statusEffect.behaviour).amount);
					GUILayout.Label(i + "-" + abilityStepWithTargetingData.abilityStep.name.ToString() + " damagePTurn: " + ((ThornsBehaviour)abilityStep.statusEffect.behaviour).amount);
					i++;
					values.Add(abilityStep.statusEffect.duration);
					GUILayout.Label(i + "-" + abilityStepWithTargetingData.abilityStep.name.ToString() + " durationRem: " + abilityStep.statusEffect.duration);
					i++;
				}
				
			}
		}

		cardSO.values = values;

		EditorStyles.textField.wordWrap = true;
		GUI.enabled = false;
		EditorGUILayout.TextArea(Utility.GetString(cardSO.description, cardSO.values, FileManager.SeparatorChar), GUILayout.Height(42));
		GUI.enabled = true;
		EditorStyles.textField.wordWrap = false;
	}*/
}
#endif