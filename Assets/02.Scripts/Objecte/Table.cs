using Unity.Netcode;
using UnityEngine;

public class Table : NetworkBehaviour
{
	[SerializeField] private Transform _putPoint;
	public NetworkVariable<bool> canPutObject = new NetworkVariable<bool>(true);
	private NetPickUp _dropObject;
	[SerializeField] private LayerMask _putLayerMask;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if (IsServer == false)
			return;

		if(Physics.Raycast(transform.position, Vector3.up,out var hit ,Mathf.Infinity, _putLayerMask))
		{
			if(hit.collider.TryGetComponent<NetPickUp>(out var dropObject))
			{
				TryPutObject(dropObject);
			}
		}

	}

	public bool TryPutObject(NetPickUp dropObject)
	{
		if (IsServer == false)
			return false;
		if (canPutObject.Value == false)
			return false;

		if( (_putLayerMask & 1 <<dropObject.gameObject.layer) > 0)
		{
			dropObject.transform.parent = null;
			dropObject.transform.position = _putPoint.position;
			canPutObject.Value = false;
			_dropObject = dropObject;
			_dropObject.onPickUp += DropObject;
			return true;
		}

		return false;
	}

	private void DropObject()
	{
		canPutObject.Value = true;
		_dropObject.onPickUp -= DropObject;
	}
}
