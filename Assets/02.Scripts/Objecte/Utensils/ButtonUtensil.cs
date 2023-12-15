using CopycatOverCooked.Datas;
using UnityEngine;

namespace CopycatOverCooked.Utensils
{
	public class ButtonUtensil : UtensilBase
	{
		public KeyCode actionKey => KeyCode.Q;
		[SerializeField] private float _actionProgress;

		public override void UpdateProgress()
		{
			if (CanCooking() == false)
				return;

			if (currentProgress == ProgressType.Progressing)
			{
				cookProgress += _actionProgress;
				if (cookProgress >= progressRecipe.cookSucessProgress)
					SurcessProgress();

			}
		}

		protected override bool CanCooking()
		{
			return currentProgress == ProgressType.Progressing;
		}

		protected override bool CanGrabable()
		{
			return false;
		}

		
	}
}
