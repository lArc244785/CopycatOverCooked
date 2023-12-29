using CopycatOverCooked.Datas;
using CopycatOverCooked.GamePlay;
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

		private Ingredient _ingredient;
		public bool isDirty
		{
			get
			{
				return _isDirty;
			}
			private set
			{
				_isDirty = value;
				onChangeIsDirty?.Invoke(value);
			}
		}
		private bool _isDirty;

		public Action<bool> onChangeIsDirty;

		[SerializeField] private Material _clearMaterial;
		[SerializeField] private Material _dirtyMaterial;
		[SerializeField] private MeshRenderer _renderer;

		private void Awake()
		{
			onChangeIsDirty += (isDirty) =>
				_renderer.material = isDirty ? _dirtyMaterial : _clearMaterial;
			
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
						else if(table.TryGetPutObject(out var putObject))
						{
							if(putObject.TryGetComponent<Ingredient>(out var ingredient))
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
						if(CanAdd(ingredient.ingerdientType.Value))
						{
							AddIngredientServerRpc(ingredient.NetworkObjectId);
						}
					}
					break;
				case InteractableType.PickUtensil:
					if(other is PickUtensil)
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
					if(otherSink.CanPlateToSink(this))
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
			if (isDirty == true)
				return false;

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

		[ServerRpc(RequireOwnership = false)]
		public void SpillIngredientServerRpc()
		{
			_ingredient?.DestoryObjectServerRpc();
			SpillIngredientClientRpc();
		}

		[ClientRpc]
		private void SpillIngredientClientRpc()
		{
			_ingredient = null;
			isDirty = true;
			Debug.Log("접시가 더러워졌습니다.");
		}

		public IngredientType GetIngereint()
		{
			if(_ingredient == null)
				return IngredientType.None;

			return _ingredient.ingerdientType.Value;
		}

		[ClientRpc]
		public void ClearPlateClientRpc()
		{
			isDirty = false;
		}
		
	}
}