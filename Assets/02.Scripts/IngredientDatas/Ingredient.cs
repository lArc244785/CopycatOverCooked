using CopycatOverCooked.Datas;
using CopycatOverCooked.GamePlay;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace CopycatOverCooked.Interaction
{
    public class Ingredient : Pickable
    {
		public  NetworkVariable<IngredientType> ingerdientType = new NetworkVariable<IngredientType>();
		[SerializeField] private Image _image;
		private GameObject _visualObject;

		public override InteractableType type => InteractableType.Ingrediant;

		private void Awake()
		{
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
			if(_visualObject != null)
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
					table.DropServerRpc(pickingClientID.Value);
					break;
				case InteractableType.Plate:

					break;
				case InteractableType.Ingrediant:
					break;
			}
		}


	}
}