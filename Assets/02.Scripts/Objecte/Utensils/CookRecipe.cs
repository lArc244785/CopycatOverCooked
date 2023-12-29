using UnityEngine;
using CopycatOverCooked.Datas;

namespace CopycatOverCooked.Untesil
{

	[CreateAssetMenu(fileName = "newCookRecipe" ,menuName = "CookRecipe")]
	public class CookRecipe : ScriptableObject
	{
		public IngredientType source;
		public IngredientType result;
	}
}
