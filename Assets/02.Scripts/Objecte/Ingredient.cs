using CopycatOverCooked.Datas;
using CopycatOverCooked.GamePlay;
using CopycatOverCooked.Untesil;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace CopycatOverCooked.Object
{
	public class Ingredient : Pickable, IAddIngredient
	{
		public NetworkVariable<IngredientType> ingerdientType = new NetworkVariable<IngredientType>();
		private GameObject _visualObject;

		private NetworkList<int> mixIngredientTypeList;

		public override InteractableType type => InteractableType.Ingredient;

		public event Action<int,IngredientType> onUpdateMixIngredinet;

 		private void Awake()
		{
			mixIngredientTypeList = new NetworkList<int>();
			ingerdientType.OnValueChanged += (prev, current) =>
			{
				UpdateVisual(current);
			};

			mixIngredientTypeList.OnListChanged += (changeEvent) =>
			{
				onUpdateMixIngredinet?.Invoke(changeEvent.Index,(IngredientType)changeEvent.Value);
			};
		}

		public void Init(IngredientType type)
		{
			if (IsServer == false)
				throw new Exception("Init Clinet Call");

			ingerdientType.Value = type;
			mixIngredientTypeList.Add((int)type);
		}
		
		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
		}


		private void UpdateVisual(IngredientType type)
		{
			if (_visualObject != null)
			{
				Destroy(_visualObject);
			}

			GameObject visualPrefab = null;
			visualPrefab = IngredientVisualDataDB.instance.GetPrefab(type);

			_visualObject = Instantiate(visualPrefab);
			_visualObject.transform.parent = transform;
			_visualObject.transform.localPosition = Vector3.zero;
		}

		protected override void OnEndInteraction(IInteractable other)
		{
			switch (other.type)
			{
				case InteractableType.TrashCan:
					DropServerRpc();
					DestoryObjectServerRpc();
					break;
				case InteractableType.Table:
					Table table = (Table)other;
					if (table.CanPutObject(type))
					{
						DropServerRpc();
						table.PutObjectServerRpc(NetworkObjectId);
					}
					else if (table.TryGetPutObject(out var putObject))
					{
						if (putObject.TryGetComponent<IAddIngredient>(out var putObjectAdd))
						{
							if (putObjectAdd.CanAdd(ingerdientType.Value))
							{
								DropServerRpc();
								putObjectAdd.AddIngredientServerRpc(NetworkObjectId);
								if (putObject.TryGetComponent<Ingredient>(out var putObjectIngredient))
								{
									DestoryObjectServerRpc();
								}
							}
						}
					}
					break;
				case InteractableType.Plate:
					Plate plate = (Plate)other;

					if (plate.CanAdd(ingerdientType.Value))
					{
						DropServerRpc();
						plate.AddIngredientServerRpc(NetworkObjectId);
					}

					break;
				case InteractableType.Ingredient:
					Ingredient otherIngredient = (Ingredient)other;
					if (otherIngredient.CanAdd(ingerdientType.Value))
					{
						DropServerRpc();
						otherIngredient.AddIngredientServerRpc(NetworkObjectId);
						DestoryObjectServerRpc();
					}
					break;
				case InteractableType.PickUtensil:
					PickUtensil pickUntensil = (PickUtensil)other;
					if (pickUntensil.CanAdd(ingerdientType.Value))
					{
						DropServerRpc();
						pickUntensil.AddIngredientServerRpc(NetworkObjectId);
					}
					break;
				case InteractableType.FixUtensil:
					FixUtensil fixUtensil = (FixUtensil)other;
					if (fixUtensil.CanAdd(ingerdientType.Value))
					{
						DropServerRpc();
						fixUtensil.AddIngredientServerRpc(NetworkObjectId);
					}
					break;
			}
		}

		public bool CanAdd(IngredientType type)
		{
			foreach (var item in mixIngredientTypeList)
			{
				if ((type & (IngredientType)item) > 0)
					return false;
			}

			if (Enum.IsDefined(typeof(IngredientType), ingerdientType.Value | type))
				return true;

			return false;
		}

		[ServerRpc(RequireOwnership = false)]
		public void AddIngredientServerRpc(ulong netObjectID)
		{
			var netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[netObjectID];
			if (netObject.TryGetComponent<Ingredient>(out var ingredient))
			{
				ingerdientType.Value |= ingredient.ingerdientType.Value;
				mixIngredientTypeList.Add((int)ingredient.ingerdientType.Value);
			}
		}

		[ServerRpc(RequireOwnership = false)]
		public void ProcessIngredientServerRpc(IngredientType type)
		{
			ingerdientType.Value = type;
			mixIngredientTypeList[0] = (int)type;
		}
	}
}