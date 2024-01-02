using CopycatOverCooked.Datas;
using CopycatOverCooked.Untesil;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CopycatOverCooked.UIs
{
	public class PickUtensilUI : MonoBehaviour
	{
		[SerializeField] private Sprite _emptySprite;
		[SerializeField] private Image _slotPrefab;
		[SerializeField] private GridLayoutGroup _slotLayerOutGroup;
		[SerializeField] private GameObject _progressBar;
		[SerializeField] private Image _progressImage;

		[SerializeField] private PickUtensil _utensil;

		private Image[] _slotImage;
		private int _slotCount;

		private float _sucessProgress;
		private float _failProgress;

		private void Awake()
		{
			_slotImage = new Image[_utensil.capacity];

			for (int i = 0; i < _slotImage.Length; i++)
			{
				var newSlot = Instantiate(_slotPrefab, _slotLayerOutGroup.transform);
				newSlot.sprite = _emptySprite;
				_slotImage[i] = newSlot;
			}

			_sucessProgress = _utensil.sucessProgress;
			_failProgress = _utensil.failProgress;

			_utensil.onAddIngredient += OnAddIngredient;
			_utensil.onRemoveAtIngredientList += OnRemoveAtIngrdient;
			_utensil.onChangeProgress += OnChangeProgress;
			_utensil.onFail += OnFail;
			_utensil.onChangeIngredinet += OnChangeIngredient;

			OnChangeProgress(0.0f);
		}

		private void OnFail()
		{
			foreach (var slot in _slotImage)
				slot.sprite = _emptySprite;

			_slotImage[0].sprite = IngredientVisualDataDB.instance.GetSprite(IngredientType.Trash);

			_slotCount = 1;
		}

		private void OnChangeProgress(float progress)
		{
			bool isActive = progress > 0.0f ? true : false;
			_progressBar.gameObject.SetActive(isActive);
			Color color = Color.green;

			float amount = progress / _sucessProgress;
			if(amount > 1.0f)
			{
  				color = Color.red;
				amount = progress / _failProgress;
			}

			_progressImage.color = color;
			_progressImage.fillAmount = amount;
		}

		private void OnRemoveAtIngrdient(int index)
		{
			for (int i = index; i < _slotImage.Length - 1; i++)
			{
				_slotImage[i].sprite = _slotImage[i + 1].sprite;
			}

			_slotImage[_slotImage.Length - 1].sprite = _emptySprite;
			_slotCount--;
		}

		private void OnAddIngredient(IngredientType type)
		{
			Sprite sprite = IngredientVisualDataDB.instance.GetSprite(type);
			_slotImage[_slotCount++].sprite = sprite;
		}

		private void OnChangeIngredient(int index, IngredientType type)
		{
			Sprite sprite = IngredientVisualDataDB.instance.GetSprite(type);
			_slotImage[index].sprite = sprite;
		}
	}
}
