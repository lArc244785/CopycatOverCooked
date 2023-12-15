using CopycatOverCooked.Datas;
using CopycatOverCooked.Utensils;
using UnityEngine;
using UnityEngine.UI;

namespace CopycatOverCooked.UIs
{
	public class UtensilUI : MonoBehaviour
	{
		[SerializeField] private Transform _gridTransform;
		private Image[] _slotImage;

		[SerializeField] private GameObject _progressBar;
		[SerializeField] private Image _progressGague;

		private float _sucessProgress;

		private UtensilBase _utensil;

		private void Start()
		{
			_slotImage = _gridTransform.GetComponentsInChildren<Image>();
			_utensil = transform.root.GetComponent<UtensilBase>();
			_utensil.onChangeRecipe += UpdateRecipe;
			_utensil.onUpdateProgress += UpdateProgress;
			_utensil.onChangeSlot += UpdateSlots;

			_progressBar.SetActive(false);
		}

		private void UpdateRecipe(RecipeElementInfo recipe)
		{
			if (recipe != null)
				_sucessProgress = recipe.cookSucessProgress;
			_progressGague.fillAmount = 0.0f;
		}

		private void UpdateProgress(ProgressType progress, float current)
		{
			if (progress != ProgressType.Progressing)
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

		private void UpdateSlots(IngredientType[] ingredients)
		{
			int i = 0;

			for (i = 0; i < _slotImage.Length; i++)
			{
				_slotImage[i].sprite = null;
			}

			if (ingredients.Length == 0)
				return;

			if (_slotImage.Length < ingredients.Length)
				throw new System.Exception($"{transform.root.gameObject.name} UI Slot Size Error");


			for (i = 0; i < ingredients.Length; i++)
			{
				_slotImage[i].sprite = IngredientSpriteDB.instance.GetSprite(ingredients[i]);
			}

		}

	}
}
