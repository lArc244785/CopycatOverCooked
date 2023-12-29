using CopycatOverCooked.Datas;
using CopycatOverCooked.Object;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CopycatOverCooked
{
    public class PlateUI : MonoBehaviour
    {
		[SerializeField] private Plate _plate;
		[SerializeField] private Transform _gridTransform;
		private Image[] _slotImage;

		private void Start()
		{
			_slotImage = _gridTransform.GetComponentsInChildren<Image>();
			//_plate.onChangeSlot += OnChangeSlot;
		}

		private void OnChangeSlot(IEnumerable<IngredientType> slots)
		{
			for (int i = 0; i < _slotImage.Length; i++)
				_slotImage[i].sprite = null;

			int index = 0;
			foreach (var slot in slots)
			{
				_slotImage[index++].sprite = IngredientVisualDataDB.instance.GetSprite(slot);
			}
		}
	}
}
