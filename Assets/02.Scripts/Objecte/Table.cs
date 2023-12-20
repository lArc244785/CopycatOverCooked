using Unity.Netcode;
using UnityEngine;
using CopycatOverCooked.GamePlay;
public class Table : NetworkBehaviour, IInteractable
{
	[SerializeField] private Transform _putPoint;

	private Pickable _putObject;

	public InteractableType type => InteractableType.Table;

	public void BeginInteraction(Interactor interactor)
	{
		PickUpObjectServerRpc(interactor.OwnerClientId);
	}

	public void EndInteraction(Interactor interactor)
	{
	}

	[ServerRpc(RequireOwnership = false)]
	private void PickUpObjectServerRpc(ulong clientID)
	{
		Interactor interactor = Interactor.spawned[clientID];

		if (_putObject != null &&
			interactor.currentInteractableNetworkObjectID.Value == Interactor.NETWORK_OBJECT_NULL_ID)
		{
			_putObject.BeginInteraction(interactor);
			interactor.currentInteractableNetworkObjectID.Value = _putObject.GetComponent<NetworkObject>().NetworkObjectId;
		}
		else
		{
			interactor.currentInteractableNetworkObjectID.Value = Interactor.NETWORK_OBJECT_NULL_ID;
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void DropPickableObjectServerRpc(ulong clientID)
	{
		var interactor = Interactor.spawned[clientID];

		if(interactor.TryCurrentGetInteractable(out var interactable))
		{
			if(interactable is Pickable)
			{
				Pickable pick = (Pickable)interactable;
				pick.DropServerRpc(clientID);
				if (pick.NetworkObject.TrySetParent(transform))
				{
					pick.transform.localPosition = _putPoint.localPosition;
					pick.transform.localRotation = Quaternion.identity;
					_putObject = pick;
				}

				interactor.currentInteractableNetworkObjectID.Value = Interactor.NETWORK_OBJECT_NULL_ID;
			}
		}
	}

}
