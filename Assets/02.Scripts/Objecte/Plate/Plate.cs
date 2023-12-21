using CopycatOverCooked.Datas;
using CopycatOverCooked.GamePlay;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Object
{
	public class Plate : Pickable, IAddIngredient
	{
		public override InteractableType type => InteractableType.Plate;
		[SerializeField] private Transform _ingredientPoint;

		private Ingredient _ingredient;

		protected override void OnEndInteraction(IInteractable other)
		{
			Interactor interactor = Interactor.spawned[pickingClientID.Value];

			switch (other.type)
			{
				case InteractableType.Table:
					if (other is Table)
					{
						Table table = (Table)other;
						table.InteractionServerRpc(pickingClientID.Value);
					}
					break;
				case InteractableType.Ingrediant:
					if (other is Ingredient)
					{
						Ingredient ingredient = (Ingredient)other;
						if(CanAdd(ingredient.ingerdientType.Value))
						{
							AddIngredientServerRpc(ingredient.NetworkObjectId);
						}
					}
					break;
			}
		}


		public bool CanAdd(IngredientType type)
		{
			if (_ingredient == null)
				return true;
			return _ingredient.CanAdd(type);
		}

		[ClientRpc]
		private void SetUpIngredientClientRpc(ulong netObjectID)
		{
			var netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[netObjectID];
			if (netObject.TryGetComponent<Ingredient>(out var ingredient))
			{
				_ingredient = ingredient;
			}
		}

		[ServerRpc(RequireOwnership =false)]
		public void AddIngredientServerRpc(ulong netObjectID)
		{
			var netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[netObjectID];
			if (_ingredient == null)
			{
				if (netObject.TrySetParent(transform))
				{
					netObject.transform.localPosition = _ingredientPoint.localPosition;
					netObject.transform.localRotation = Quaternion.identity;
					SetUpIngredientClientRpc(netObjectID);
				}
			}
			else
			{
				_ingredient.AddIngredientServerRpc(netObjectID);
				netObject.Despawn();
			}
		}
	}
}