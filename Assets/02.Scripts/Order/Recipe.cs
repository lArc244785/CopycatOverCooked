// Recipe.cs
using CopycatOverCooked.Datas;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class Recipe : ScriptableObject
{
    public IngredientType food;
    public List<RecipeElementInfo> elements;
}
