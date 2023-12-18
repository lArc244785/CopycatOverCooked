// Recipe.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class Recipe : ScriptableObject
{
    public List<RecipeElement> elementsList;

    public Dictionary<RecipeElement, int> Init()
    {
        Dictionary<RecipeElement, int> elementTable = new Dictionary<RecipeElement, int>();
        foreach (var element in elementsList)
        {
            elementTable.Add(element, element.amount);
        }
        return elementTable;
    }
}
