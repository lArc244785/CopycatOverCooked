using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;
using System;

namespace CopycatOverCooked.Datas
{
	[CreateAssetMenu(fileName = "NewIngredientVisualData", menuName = "AssetDatas/IngredientVisualData")]
	public class IngredientVisualData : ScriptableObject
	{
		[Serializable]
		private struct IngredientVisualElementData
		{
			public IngredientType key;
			public GameObject prefab;
		}


		[SerializeField] private List<IngredientVisualElementData> _datas;

		public Dictionary<IngredientType, GameObject> ingredientVisualTable;
		public void Init()
		{
			ingredientVisualTable = new();
			foreach(var data in _datas)
			{
				ingredientVisualTable.Add(data.key, data.prefab);
			}
		}
	}
}
