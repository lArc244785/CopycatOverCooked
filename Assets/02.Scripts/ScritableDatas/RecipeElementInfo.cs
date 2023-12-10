using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;

namespace CopycatOverCooked.Datas
{
	[CreateAssetMenu(fileName = "NewRecipeElementInfoData", menuName = "AssetDatas/RecipeElementInfo")]
	public class RecipeElementInfo : ScriptableObject
	{
		[field: SerializeField] public IngredientType resource { private set; get; }
		[field: SerializeField] public UtensilType utensil { private set; get; }
		[field: SerializeField] public IngredientType result { private set; get; }
		[field: SerializeField]public float cookSucessProgress { private set; get; }
		[field: SerializeField]public float cookFailProgress { private set; get; }
	}
}
