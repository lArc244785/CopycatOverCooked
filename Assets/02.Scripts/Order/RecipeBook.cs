using CopycatOverCooked.Datas;
using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public static RecipeBook instance;

    public Recipe this[IngredientType name] => _recipes[name];
    private Dictionary<IngredientType, Recipe> _recipes;
    [SerializeField] private List<Recipe> _recipeList;

    private void Awake()
    {
        instance = this;

        _recipes = new Dictionary<IngredientType, Recipe>();
        foreach (var data in _recipeList)
        {
            _recipes.Add(data.type, data);
        }
        _recipeList = null;
    }
}