using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace CopycatOverCooked.Datas
{
    public class Ingredient : NetworkBehaviour
    {
		public  NetworkVariable<IngredientType> type = new NetworkVariable<IngredientType>();
		[SerializeField] private Image _image;
		private GameObject _visualObject;


		private void Awake()
		{
			type.OnValueChanged += (prev, current) =>
			{
				UpdateSprite(current);
				UpdateVisual(current);
			};
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			UpdateVisual(type.Value);
			UpdateSprite(type.Value);
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

	}
}