using UnityEngine;
using UnityEngine.UI;
using CopycatOverCooked.Datas;
using System.Collections.Generic;
using Unity.Netcode;

public class TestPlate : NetworkBehaviour
{
	[SerializeField] private Transform _grid;
	private Image[] _slots;

	/// <summary>
	/// IngredientType
	/// </summary>
	public NetworkList<int> inputIngredients;
	[field: SerializeField] public int capacity { private set; get; }


	private void Awake()
	{
		_slots = _grid.GetComponentsInChildren<Image>();
		inputIngredients = new NetworkList<int>();
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		inputIngredients.OnListChanged += OnChangeSlot;
		for (int i = 0; i < inputIngredients.Count; i++)
		{
			var ingredientSprite = IngredientVisualDataDB.instance.GetSprite((IngredientType)inputIngredients[i]);
			_slots[i].sprite = ingredientSprite;
		}
	}


	public void InputIngredient(IngredientType[] ingredients)
	{
		for(int i = 0; i < ingredients.Length; i++)
		{
			if (inputIngredients.Count == capacity)
				break;
			inputIngredients.Add((int)ingredients[i]);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void SpillTrashServerRpc()
	{
		Spill();
	}

	public IngredientType[] Spill()
	{
		IngredientType[] ingredients = new IngredientType[inputIngredients.Count];
		for (int i = 0; i < ingredients.Length; i++)
			ingredients[i] = (IngredientType)inputIngredients[i];

		inputIngredients.Clear();

		return ingredients;
	}

	private void OnChangeSlot(NetworkListEvent<int> changeEvent)
	{
		for(int i = 0; i < _slots.Length; i++)
		{
			_slots[i].sprite = null;
		}

		if (changeEvent.Type == NetworkListEvent<int>.EventType.Clear)
			return;

		for(int i = 0; i < inputIngredients.Count; i++)
		{
			var ingredientSprite = IngredientVisualDataDB.instance.GetSprite((IngredientType)inputIngredients[i]);
			_slots[i].sprite = ingredientSprite;
		}

		var sprite = IngredientVisualDataDB.instance.GetSprite((IngredientType)changeEvent.Value);
		_slots[changeEvent.Index].sprite = sprite;
	}

}
