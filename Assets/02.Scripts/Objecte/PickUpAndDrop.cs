using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAndDrop : MonoBehaviour
{
	public void PickUp(Transform parent)
	{
		transform.parent = parent;
		transform.localPosition = Vector3.zero;
	}

	public void Drop(Vector3 position)
	{
		transform.parent = null;
		transform.localPosition = position;
	}
}
