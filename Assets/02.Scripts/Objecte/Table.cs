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
		if (_putObject == null)
		{
			interactor.currentInteractable = null;
			return;
		}

		interactor.currentInteractable = _putObject;
		_putObject.BeginInteraction(interactor);
	}

	public void EndInteraction(Interactor interactor)
	{
	}

	[ServerRpc(RequireOwnership = false)]
	public void DropServerRpc(ulong clientID)
	{
		var interactor = Interactor.spawned[clientID];

		if(interactor.currentInteractable is Pickable)
		{
			Pickable pick = (Pickable)interactor.currentInteractable;
			pick.DropServerRpc(clientID);
			if(pick.NetworkObject.TrySetParent(transform))
			{
				pick.transform.localPosition = _putPoint.localPosition;
				_putObject = pick;
			}
		}
	}

}
