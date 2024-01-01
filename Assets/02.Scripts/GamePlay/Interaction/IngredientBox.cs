using CopycatOverCooked.Datas;
using CopycatOverCooked.GamePlay;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Object
{
	public class IngredientBox : NetworkBehaviour, IInteractable
	{
		public InteractableType type => InteractableType.IngrediantBox;
		[SerializeField] private IngredientType spawnType;
		[SerializeField] private Ingredient prefab;
		public void BeginInteraction(Interactor interactor)
		{
			SpawnIngredientServerRpc(interactor.OwnerClientId);
		}

		public void EndInteraction(Interactor interactor)
		{
		}

		[ServerRpc(RequireOwnership = false)]
		private void SpawnIngredientServerRpc(ulong clientID)
		{
			Interactor interactor = Interactor.spawned[clientID];

			var ingredientObject = Instantiate(prefab, transform.position, Quaternion.identity);	
			var netObject = ingredientObject.GetComponent<NetworkObject>();
			netObject.Spawn();
			ingredientObject.Init(spawnType);

			ingredientObject.GetComponent<IInteractable>().BeginInteraction(interactor);
			interactor.currentInteractableNetworkObjectID.Value = netObject.NetworkObjectId;
		}
	}
}
