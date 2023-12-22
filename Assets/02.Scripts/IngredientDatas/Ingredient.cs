using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace CopycatOverCooked.Datas
{
    public class Ingredient : NetworkBehaviour
    {
		public IngredientType type;
		[SerializeField] private Image _image;


		private void Awake()
		{
			UpdateSprite(type);
		}

        private void UpdateSprite(IngredientType type)
        {
			var sprite = IngredientSpriteDB.instance.GetSprite(type);
			_image.sprite = sprite;
		}
    }
}