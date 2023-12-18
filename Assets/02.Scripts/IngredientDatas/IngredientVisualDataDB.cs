using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;
using System;

namespace CopycatOverCooked.Datas
{

	public class IngredientVisualDataDB : MonoBehaviour
	{
		public static IngredientVisualDataDB instance { private set; get; }

		[SerializeField] private IngredientUISpriteData _spriteData;
		[SerializeField] private IngredientVisualData _visualData;
		private void Awake()
		{
			_spriteData.Init();
			_visualData.Init();
			instance = this;
		}

		public Sprite GetSprite(IngredientType type)
		{
			if (_spriteData.ingredientSpriteTable.ContainsKey(type))
				return _spriteData.ingredientSpriteTable[type];
			else
				return null;
		}

		public GameObject GetPrefab(IngredientType type)
		{
			if (_visualData.ingredientVisualTable.ContainsKey(type))
				return _visualData.ingredientVisualTable[type];
			else
				return _visualData.ingredientVisualTable[IngredientType.None];
		}
	}
}

