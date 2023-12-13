using CopycatOverCooked.Datas;
using CopycatOverCooked.Utensils;
using UnityEngine;

namespace CopycatOverCooked.NetWork.Untesils
{
	public class NetTimeUtensil : NetUtensillBase
	{
		[SerializeField] private string _progressOnTag;
		[SerializeField] private bool _isProgressable = false;

		private void Update()
		{
			UpdateProgress();
		}

		public override void UpdateProgress()
		{
			if (IsServer == false)
				return;
			if (CanCooking() == false || _isProgressable == false)
				return;

			switch ((ProgressType)progressType.Value)
			{
				case ProgressType.Progressing:
					if (progress.Value >= GetCurrentRecipe().cookSucessProgress)
						Sucess();
					break;
				case ProgressType.Sucess:
					if (progress.Value >= GetCurrentRecipe().cookFailProgress)
						Fail();
					break;
			}
			progress.Value += Time.deltaTime;
		}

		protected override bool CanCooking()
		{
			return GetCurrentRecipe() != null &&
				(ProgressType)progressType.Value == ProgressType.Progressing ||
				(ProgressType)progressType.Value == ProgressType.Sucess;
		}

		protected override bool CanGrabable()
		{
			return true;
		}

		private void Sucess()
		{
			var result = (int)GetCurrentRecipe().result;

			for (int i = 0; i < inputIngredients.Count; i++)
			{
				inputIngredients[i] = result;
			}
			progressType.Value = (int)ProgressType.Sucess;
		}

		private void Fail()
		{
			inputIngredients.Clear();
			inputIngredients.Add((int)IngredientType.Trash);
			progressType.Value = (int)ProgressType.Fail;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (IsServer == false)
				return;

			if (other.gameObject.tag.Equals(_progressOnTag))
				_isProgressable = true;
		}

		private void OnTriggerExit(Collider other)
		{
			if (IsServer == false)
				return;

			if (other.gameObject.tag.Equals(_progressOnTag))
				_isProgressable = false;
		}
	}
}
