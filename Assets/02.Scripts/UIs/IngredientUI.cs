using CopycatOverCooked.Datas;
using CopycatOverCooked.Object;

using UnityEngine;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
{
	[SerializeField] private Image[] icons;
	[SerializeField] private GameObject _slotGameObject;
	private void Awake()
	{
		Ingredient ingredient = transform.root.GetComponent<Ingredient>();
		ingredient.onUpdateMixIngredinet += OnUpdateMixIngreident;
		ingredient.onSetVisableUI += OnSetVisableUI;

		foreach (var icon in icons)
		{
			icon.gameObject.SetActive(false);
		}
	}

	private void OnUpdateMixIngreident(int index, IngredientType type)
	{
		icons[index].gameObject.SetActive(true);
		icons[index].sprite = IngredientVisualDataDB.instance.GetSprite(type);
	}

	private void OnSetVisableUI(bool isVisable)
	{
		_slotGameObject.SetActive(isVisable);
	}

}
