using CopycatOverCooked;
using CopycatOverCooked.GamePlay;
using CopycatOverCooked.NetWork;
using CopycatOverCooked.Object;
using Unity.Netcode;
using UnityEngine;

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
			_putObject = null;
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void InteractionServerRpc(ulong clientID)
	{
		var interactor = Interactor.spawned[clientID];

		if (_putObject == null)
		{
			if (interactor.TryCurrentGetInteractable(out var interactable))
			{
				if (interactable is Pickable)
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
		else if (interactor.TryGet(interactor.currentInteractableNetworkObjectID.Value, out var inputNetObject))
		{
			//올려져 있는 오브젝트가 재료인 경우
			if (_putObject is Ingredient)
			{
				Ingredient putIngredient = (Ingredient)_putObject;

				//들어온 오브젝트가 재료인 경우
				if (inputNetObject.TryGetComponent<Ingredient>(out var ingredient))
				{
					IAddIngredient addPutIngredient = putIngredient;
					if (addPutIngredient.CanAdd(ingredient.ingerdientType.Value))
					{
						ingredient.DropServerRpc();
						addPutIngredient.AddIngredientServerRpc(ingredient.NetworkObjectId);
						inputNetObject.Despawn();
						interactor.currentInteractableNetworkObjectID.Value = Interactor.NETWORK_OBJECT_NULL_ID;
					}
				}
				//들어온 오브젝트가 접시인경우
				else if(inputNetObject.TryGetComponent<Plate>(out var plate))
				{
					IAddIngredient addPlate = plate;
					if(addPlate.CanAdd(putIngredient.ingerdientType.Value))
					{
						addPlate.AddIngredientServerRpc(putIngredient.NetworkObjectId);
						_putObject.DestoryObjectServerRpc();
						_putObject = null;
					}
				}

				
			}
			//올려져 있는 오브젝트가 접시인 경우
			if(_putObject is Plate)
			{
				//들어온 오브젝트가 재료인 경우
				if(inputNetObject.TryGetComponent<Ingredient>(out var ingredient))
				{
					IAddIngredient addPlate = (Plate)_putObject;
					if(addPlate.CanAdd(ingredient.ingerdientType.Value))
					{
						ingredient.DropServerRpc();
						addPlate.AddIngredientServerRpc(ingredient.NetworkObjectId);
						interactor.currentInteractableNetworkObjectID.Value = Interactor.NETWORK_OBJECT_NULL_ID;
					}
				}
			}
		}
	}

}
