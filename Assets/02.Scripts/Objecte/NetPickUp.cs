using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetPickUp : NetworkBehaviour
{
	public event Action onPickUp;

	public void PickUp(Transform parent, Vector3 localPos)
	{
		if (IsServer)
		{
			transform.parent = parent;
			transform.localPosition = localPos;
			onPickUp?.Invoke();
		}
	}

}
