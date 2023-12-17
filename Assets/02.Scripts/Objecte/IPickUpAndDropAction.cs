using UnityEngine;

namespace CopycatOverCooked.Interaction
{
	internal interface IPickUpAndDropAction
	{
		void PickUp(Transform parent, Vector3 localPos);
		void Drop(Vector3 position);
	}
}
