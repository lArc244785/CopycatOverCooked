using CopycatOverCooked.Datas;
using System.Collections.Generic;
using UnityEngine;

namespace CopycatOverCooked.Orders
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Recipes/Recipe")]
    public class Recipe : ScriptableObject
    {
        [SerializeField]
        private List<RecipeElementInfo> elements = new List<RecipeElementInfo>();

        public IngredientType result
        {
            get
            {
                IngredientType type = IngredientType.None;
                foreach (var element in elements)
                    type |= element.ingredient;
                return type;
            }
        }

        public List<RecipeElementInfo> Elements
        {
            get { return elements; }
        }

        [System.Serializable]
        public class RecipeElementInfo
        {
            public IngredientType ingredient;
            public int quantity;
        }
    }
}
