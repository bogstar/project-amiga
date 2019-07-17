using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI
{
	public class VersionUIDisplay : MonoBehaviour
	{
		Text text;

		void Start()
		{
			text = GetComponent<Text>();
			text.text = GameManager.CurrentGameVersion;
		}
	}
}