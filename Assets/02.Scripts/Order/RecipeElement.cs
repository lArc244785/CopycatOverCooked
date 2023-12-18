using CopycatOverCooked.Datas;
using UnityEngine;

[CreateAssetMenu(fileName = "New RecipeElement", menuName = "Recipe Element")]
public class RecipeElement : ScriptableObject
{
    public RecipeElementInfo info;
    public int amount;
}