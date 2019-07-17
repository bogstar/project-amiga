using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amiga.UI
{
	public abstract class SceneSettings : MonoBehaviour
	{
		[Header("Prefabs")]
		[SerializeField]
		protected GameObject versionPrefab;

		protected Stack<HUDPanel> states;

		protected virtual void Awake()
		{
			states = new Stack<HUDPanel>();
		}

		protected IEnumerator TryOpen(HUDPanel panel)
		{
			while(panel.gameObject.activeSelf == false)
			{
				panel.gameObject.SetActive(true);
				yield return null;
			}
		}

		public virtual void HideTopPanel()
		{
			if(states.Count > 0)
			{
				HUDPanel panel = states.Pop();
				panel.gameObject.SetActive(false);
			}
		}

		public void ShowPanel(HUDPanel panel)
		{
			states.Push(panel);
			StartCoroutine(TryOpen(panel));
		}
	}
}