using CopycatOverCooked.Datas;
using CopycatOverCooked.GamePlay;
using CopycatOverCooked.NetWork;
using CopycatOverCooked.Untesil;
using System;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Object
{
	public class Plate : Pickable, IAddIngredient
	{
		public override InteractableType type => InteractableType.Plate;
		[SerializeField] private Transform _ingredientPoint;

		private NetworkVariable<ulong> _ingredientNetworkId =
			new NetworkVariable<ulong>(NetworkBehaviourExtensions.NULL_NETWORK_OBJECT_ID);

		private bool TryGetIngredient(out Ingredient ingredient)
		{
			ingredient = null;
			if (_ingredientNetworkId.Value == NetworkBehaviourExtensions.NULL_NETWORK_OBJECT_ID)
			{
				return false;
			}
			else if (this.TryGet(_ingredientNetworkId.Value, out var ingredinetObject))
			{
				ingredient = ingredinetObject.GetComponent<Ingredient>();
				return true;
			}

			return false;
		}

		public NetworkVariable<bool> isDirty = new NetworkVariable<bool>(false);

		public Action<bool> onChangeIsDirty;

		[SerializeField] private Mesh _clearMesh;
		[SerializeField] private Mesh _dirtyMesh;
		[SerializeField] private MeshFilter _meshFilter;

		private void Awake()
		{
			isDirty.OnValueChanged += (prev, current) => onChangeIsDirty?.Invoke(current);
			onChangeIsDirty += (isDirty) =>
				_meshFilter.mesh = isDirty ? _dirtyMesh : _clearMesh;

		}



		protected override void OnEndInteraction(IInteractable other)
		{
			Interactor interactor = Interactor.spawned[pickingClientID.Value];

			switch (other.type)
			{
				case InteractableType.Table:
					if (other is Table)
					{
						Table table = (Table)other;
						if (table.CanPutObject(type))
						{
							DropServerRpc();
							table.PutObjectServerRpc(NetworkObjectId);
						}
						else if (table.TryGetPutObject(out var putObject))
						{
							if (putObject.TryGetComponent<Ingredient>(out var ingredient))
							{
								if (CanAdd(ingredient.ingerdientType.Value))
								{
									table.PopPutObjectServerRpc();
									AddIngredientServerRpc(ingredient.NetworkObjectId);
								}
							}
						}
					}
					break;
				case InteractableType.Ingredient:
					if (other is Ingredient)
					{
						Ingredient ingredient = (Ingredient)other;
						if (CanAdd(ingredient.ingerdientType.Value))
						{
							AddIngredientServerRpc(ingredient.NetworkObjectId);
						}
					}
					break;
				case InteractableType.PickUtensil:
					if (other is PickUtensil)
					{
						PickUtensil pickUtensil = (PickUtensil)other;
						pickUtensil.DropIngredientToPlateServerRpc(pickingClientID.Value);
					}
					break;
				case InteractableType.TrashCan:
					SpillIngredientServerRpc();
					break;
				case InteractableType.Sink:
					Sink otherSink = (Sink)other;
					if (otherSink.CanPlateToSink(this))
					{
						DropServerRpc();
						otherSink.PlateToSinkServerRpc(NetworkObjectId);
					}
					break;
				case InteractableType.ServingConvayer:
					DropServerRpc();
					ServingConvayer convayer = (ServingConvayer)other;
					convayer.SendOrder(this);
					break;
			}
		}


		public bool CanAdd(IngredientType type)
		{
			if (isDirty.Value == true)
				return false;

			if (_ingredientNetworkId.Value == NetworkBehaviourExtensions.NULL_NETWORK_OBJECT_ID)
				return true;

			if (TryGetIngredient(out var ingredient))
			{
				return ingredient.CanAdd(type);
			}

			return false;
		}


		[ServerRpc(RequireOwnership = false)]
		public void AddIngredientServerRpc(ulong netObjectID)
		{
			var netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[netObjectID];
			//접	시에 재료가 있는 경우
			if (TryGetIngredient(out var baseIngredient))
			{
				if (netObject.TryGetComponent<Ingredient>(out var input))
				{
					if (baseIngredient.CanAdd(input.ingerdientType.Value))
					{
						baseIngredient.AddIngredientServerRpc(input.NetworkObjectId);
						netObject.Despawn();
					}
				}
			}
			//없는 경우
			else if (netObject.TrySetParent(transform))
			{
				netObject.transform.localPosition = _ingredientPoint.localPosition;
				netObject.transform.localRotation = _ingredientPoint.localRotation;
				_ingredientNetworkId.Value = netObjectID;
			}
		}

		[ServerRpc(RequireOwnership = false)]
		public void SpillIngredientServerRpc()
		{
			if (TryGetIngredient(out var ingredient))
			{
				ingredient.DestoryObjectServerRpc();
				_ingredientNetworkId.Value = NetworkBehaviourExtensions.NULL_NETWORK_OBJECT_ID;
				isDirty.Value = true;
			}
		}


		public IngredientType GetIngereint()
		{
			if (TryGetIngredient(out var ingredinet))
				return ingredinet.ingerdientType.Value;

			return IngredientType.None;
		}

	}
}