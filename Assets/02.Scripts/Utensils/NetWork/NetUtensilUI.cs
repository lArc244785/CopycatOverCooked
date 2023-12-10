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

		private float _sucessProgress;

		private NetUtensillBase _utensil;

		private void Start()
		{
			_slotImage = _gridTransform.GetComponentsInChildren<Image>();
			_utensil = transform.root.GetComponent<NetUtensillBase>();
			_utensil.onChangeRecipe += UpdateRecipe;
			_utensil.onUpdateProgress += UpdateProgress;
			_utensil.onUpdateSlot += UpdateSlots;

			_progressBar.SetActive(false);
		}

		private void UpdateRecipe(RecipeElementInfo recipe)
		{
			if (recipe != null)
				_sucessProgress = recipe.cookSucessProgress;
			_progressGague.fillAmount = 0.0f;
		}

		private void UpdateProgress(ProgressState progress, float current)
		{
			if (progress != ProgressState.Progressing)
			{
				_progressBar.SetActive(false);
				return;
			}
			else
			{
				_progressBar.SetActive(true);
			}
			float fillAmount = Mathf.Clamp(current / _sucessProgress, 0.0f, 1.0f);
			_progressGague.fillAmount = fillAmount;
		}

		private void UpdateSlots(IEnumerator<int> enumerator)
		{
			int i = 0;

			for (i = 0; i < _slotImage.Length; i++)
			{
				_slotImage[i].sprite = null;
			}

			i = 0;
			enumerator.Reset();
			while (enumerator.MoveNext())
			{
				_slotImage[i++].sprite = IngredientSpriteDB.instance.GetSprite((IngredientType)enumerator.Current);
			}
			enumerator.Reset();
		}

	}
}
