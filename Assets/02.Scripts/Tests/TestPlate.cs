using CopycatOverCooked.Utensils;
using UnityEngine;
using UnityEngine.UI;
using CopycatOverCooked.Datas;

public class TestPlate : MonoBehaviour
{
	[SerializeField] private UtensilBase utensill;
	[SerializeField] private Transform _grid;
	private Image[] _slots;

	private void Start()
	{
		_slots = _grid.GetComponentsInChildren<Image>();
	}

	public void TestSoil()
	{
		if (utensill.TrySpillToPlate(out var ingredients))
		{
			for(int i = 0; i < _slots.Length; i++)
			{
				_slots[i].sprite = null;
			}

			for (int i = 0; i < ingredients.Length; i++)
			{
				_slots[i].sprite = IngredientSpriteDB.instance.GetSprite(ingredients[i]);
			}
		}
	}
}
