using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amiga.UI
{
	public class HUDPanel : MonoBehaviour
	{
		protected virtual void Start()
		{
			gameObject.SetActive(false);
		}
	}
}