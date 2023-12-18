using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class NetPickUp : NetworkBehaviour
{
	public event Action onPickUp;
	public NetworkVariable<bool> canPickUP = new NetworkVariable<bool>(true);

	public void PickUp(Transform parent, Vector3 localPos)
	{
		if (IsServer && canPickUP.Value)
		{
			onPickUp?.Invoke();
			transform.parent = parent;
			transform.localPosition = localPos;
		}
	}

}
