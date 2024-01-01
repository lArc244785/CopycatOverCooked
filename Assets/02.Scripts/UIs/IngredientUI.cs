using CopycatOverCooked.Datas;
using CopycatOverCooked.Object;

using UnityEngine;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
{
	[SerializeField] private Image[] icons;

	private void Awake()
	{
		Ingredient ingredient = transform.root.GetComponent<Ingredient>();
		ingredient.onUpdateMixIngredinet += OnUpdateMixIngreident;

		foreach(var icon in icons)
		{
			icon.gameObject.SetActive(false);
		}
	}

	private void OnUpdateMixIngreident(int index, IngredientType type)
	{
		icons[index].gameObject.SetActive(true);
		icons[index].sprite = IngredientVisualDataDB.instance.GetSprite(type);
	}

}
