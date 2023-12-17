using CopycatOverCooked.Datas;
using CopycatOverCooked.NetWork.Untesils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CopycatOverCooked.UIs
{
	public class NetUtensilUI : MonoBehaviour
	{
		[SerializeField] private Transform _gridTransform;
		private Image[] _slotImage;

		[SerializeField] private GameObject _progressBar;
		[SerializeField] private Image _progressGague;

		private NetUtensillBase _utensil;

		private void Start()
		{
			_utensil = transform.root.GetComponent<NetUtensillBase>();
			_utensil.onChangeProgress += UpdateProgress;

			if (_gridTransform != null)
			{
				_slotImage = _gridTransform.GetComponentsInChildren<Image>();
				_utensil.onChangeSlot += UpdateSlots;
			}

			_progressBar.SetActive(false);
		}



		private void UpdateProgress(float current, float surcessProgress)
		{
			Debug.Log(current);
			if (current <= 0.0f)
			{
				_progressBar.SetActive(false);
				return;
			}

			_progressBar.SetActive(true);

			float fillAmount = Mathf.Clamp(current / surcessProgress, 0.0f, 1.0f);
			_progressGague.fillAmount = fillAmount;
		}

		private void UpdateSlots(IEnumerable<IngredientType> inputIngredient)
		{
			int i = 0;

			for (i = 0; i < _slotImage.Length; i++)
			{
				_slotImage[i].sprite = null;
			}

			if (inputIngredient == null)
				return;

			i = 0;

			foreach (var item in inputIngredient)
			{
				_slotImage[i].sprite = IngredientVisualDataDB.instance.GetSprite(item);
				i++;
			}
		}

	}
}
