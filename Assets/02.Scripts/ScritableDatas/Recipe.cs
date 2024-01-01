using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;

namespace CopycatOverCooked.Datas
{
	[CreateAssetMenu(fileName = "NewRecipeData", menuName = "AssetDatas/Recipe")]
	public class Recipe : ScriptableObject
	{
		public List<RecipeElementInfo> elements;
		public IngredientType result;
	}
}

