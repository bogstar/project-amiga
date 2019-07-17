using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Inventory
{
	public class RemovableArea : MonoBehaviour
	{
		public Image highlightable;
		public UISlot slot;
		public IRemovable removable;
	}
}