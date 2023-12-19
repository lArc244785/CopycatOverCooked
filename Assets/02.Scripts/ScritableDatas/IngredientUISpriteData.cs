using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;
using System;

namespace CopycatOverCooked.Datas
{
	[CreateAssetMenu(fileName = "NewIngredientUIData", menuName = "AssetDatas/IngredientUIData")]
	public class IngredientUISpriteData : ScriptableObject
	{
		[Serializable]
		private struct IngredientUIElementData
		{
			public IngredientType key;
			public Sprite sprite;
		}

		[SerializeField] private List<IngredientUIElementData> _datas;

		public Dictionary<IngredientType, Sprite> ingredientSpriteTable;
		public void Init()
		{
			ingredientSpriteTable = new();
			foreach(var data in _datas)
			{
				ingredientSpriteTable.Add(data.key, data.sprite);
			}
		}
	}
}
