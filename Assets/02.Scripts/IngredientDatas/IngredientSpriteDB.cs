using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;

namespace CopycatOverCooked.Datas
{

	public class IngredientSpriteDB : MonoBehaviour
	{
		public static IngredientSpriteDB instance { private set; get; }

		[SerializeField] private IngredientUISpriteData _data;
		private void Awake()
		{
			_data.Init();
			instance = this;
		}

		public Sprite GetSprite(IngredientType type)
		{
			if (_data.ingredientSpriteTable.ContainsKey(type))
				return _data.ingredientSpriteTable[type];
			else
				return null;
		}
	}
}

