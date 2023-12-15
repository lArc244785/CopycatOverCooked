using Unity.Netcode;
using UnityEngine;

public class Table : NetworkBehaviour
{
	[SerializeField] private Transform _putPoint;
	public NetworkVariable<bool> canPutObject = new NetworkVariable<bool>(true);
	private NetPickUp _dropObject;

	public void PutObject(NetPickUp dropObject)
	{
		if (IsServer == false)
			return;
		if (canPutObject.Value == false)
			return;

		dropObject.transform.parent = null;
		dropObject.transform.position = _putPoint.position;
		canPutObject.Value = false;
		_dropObject = dropObject;
		_dropObject.onPickUp += DropObject;
	}

	private void DropObject()
	{
		canPutObject.Value = true;
		_dropObject.onPickUp -= DropObject;
	}
}
