using CopycatOverCooked.Datas;
using CopycatOverCooked.GamePlay;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace CopycatOverCooked.Object
{
	public class Ingredient : Pickable, IAddIngredient
	{
		public NetworkVariable<IngredientType> ingerdientType = new NetworkVariable<IngredientType>();
		[SerializeField] private Image _image;
		private GameObject _visualObject;

		private NetworkList<int> mixIngredientList;

		public override InteractableType type => InteractableType.Ingrediant;

		public void Init(IngredientType type)
		{
			if (IsServer == false)
				throw new Exception("Init Clinet Call");

			ingerdientType.Value = type;
			mixIngredientList.Add((int)type);
		}

		[ServerRpc(RequireOwnership = false)]
		private void AddIngredientServerRpc(int type)
		{
			ingerdientType.Value |= (IngredientType)type;
			mixIngredientList.Add(type);
		}

		private void Awake()
		{
			mixIngredientList = new NetworkList<int>();
			ingerdientType.OnValueChanged += (prev, current) =>
			{
				UpdateSprite(current);
				UpdateVisual(current);
			};
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			UpdateVisual(ingerdientType.Value);
			UpdateSprite(ingerdientType.Value);
		}

		private void UpdateSprite(IngredientType type)
		{
			var sprite = IngredientVisualDataDB.instance.GetSprite(type);
			_image.sprite = sprite;
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
					//Todo - ThrashCan.DestoryObject
					break;
				case InteractableType.Table:
					Table table = (Table)other;
					table.InteractionServerRpc(pickingClientID.Value);
					break;
				case InteractableType.Plate:

					break;
				case InteractableType.Ingrediant:
					{
						Ingredient otherIngredient = (Ingredient)other;
						if (CanAdd(otherIngredient.ingerdientType.Value))
						{
							AddIngredientServerRpc(otherIngredient.NetworkObjectId);
							DestoryObjectServerRpc();
						}
					}
					break;
			}
		}

		public bool CanAdd(IngredientType type)
		{
			foreach (var item in mixIngredientList)
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
				mixIngredientList.Add((int)ingredient.ingerdientType.Value);
			}
		}
	}
}