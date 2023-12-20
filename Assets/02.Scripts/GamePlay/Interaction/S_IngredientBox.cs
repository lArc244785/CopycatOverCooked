using CopycatOverCooked.Datas;
using CopycatOverCooked.Interaction;
using CopycatOverCooked.NetWork;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Android;

namespace CopycatOverCooked.GamePlay
{
	public class S_IngredientBox : NetworkBehaviour, IInteractable
	{
		public InteractableType type => InteractableType.IngrediantBox;
		[SerializeField] private IngredientType spawnType;
		[SerializeField] private Ingredient prefab;
		[SerializeField] private TextMeshProUGUI m_boxNameText;

		private void Awake()
		{
			m_boxNameText.text = spawnType.ToString();
		}

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
			ingredientObject.ingerdientType.Value = spawnType;

			interactor.currentInteractableNetworkObjectID.Value = netObject.NetworkObjectId;
			netObject.GetComponent<IInteractable>().BeginInteraction(interactor);
		}
	}
}
