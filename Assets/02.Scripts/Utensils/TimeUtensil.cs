using CopycatOverCooked.Datas;
using UnityEngine;

namespace CopycatOverCooked.Utensils
{
	public class TimeUtensil : UtensilBase
	{
		[SerializeField] private LayerMask _detectObjectMask;
		[SerializeField] private string cookableDetectTag;
		private string _detectTag;
		private bool isBurring = false;

		protected override bool CanCooking()
		{
			return cookableDetectTag.Equals(_detectTag);
		}

		protected override bool CanGrabable()
		{
			return currentProgress != ProgressType.Progressing && isBurring == false;
		}


		public override void UpdateProgress()
		{
			switch (currentProgress)
			{
				case ProgressType.Progressing:
					cookProgress += Time.deltaTime;
					if (cookProgress >= progressRecipe.cookSucessProgress)
						SurcessProgress();
					break;

				case ProgressType.Sucess:
					cookProgress += Time.deltaTime;
					if (cookProgress >= progressRecipe.cookFailProgress)
						FailProgress();
					break;
			}
		}

		private void FailProgress()
		{
			slots.Clear();
			slots.Add(IngredientType.Trash);
			//todo fire

			currentProgress = ProgressType.Fail;
			return;
		}

		private void Update()
		{
			if (CanCooking() == false)
				return;

			UpdateProgress();
		}
	}
}
