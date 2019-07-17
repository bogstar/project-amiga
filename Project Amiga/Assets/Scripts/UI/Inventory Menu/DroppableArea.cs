using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amiga.UI.Inventory
{
	public class DroppableArea : MonoBehaviour
	{
		public Image highlightable;
		public UISlot slot;
		public IDroppable droppable;
	}
}