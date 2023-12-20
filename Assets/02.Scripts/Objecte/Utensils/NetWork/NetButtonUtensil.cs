using CopycatOverCooked.Datas;
using CopycatOverCooked.Interaction;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.NetWork.Untesils
{
	public class NetButtonUtensil : NetUtensillBase
	{
		[SerializeField] private float _actionProgress;
		[SerializeField] private Transform _ingredientPosition;
		private Ingredient _ingredient;

		public override void UpdateProgress()
		{

			if (CanCooking() == false)
				return;

			if ((ProgressState)progressType.Value == ProgressState.Progressing)
			{
				progress.Value += _actionProgress;
				if (progress.Value >= GetCurrentRecipe().cookSucessProgress)
					Sucess();
			}
		}

		protected override bool CanCooking()
		{
			return (ProgressState)progressType.Value == ProgressState.Progressing;
		}

		protected override bool CanGrabable()
		{
			return false;
		}

		public override bool TryAddResource(Ingredient resource)
		{
			if (_ingredient != null)
				return false;
			if (base.TryAddResource(resource) == false)
				return false;

			_ingredient = resource;
			_ingredient.transform.parent = transform;
			_ingredient.transform.localPosition = _ingredientPosition.localPosition;
			_ingredient.GetComponent<NetPickUp>().canPickUP.Value = false;

			return true;
		}

		public override void Sucess()
		{
			base.Sucess();
			_ingredient.ingerdientType.Value = GetCurrentRecipe().result;
			_ingredient.GetComponent<NetPickUp>().canPickUP.Value = true;
			_ingredient.GetComponent<NetPickUp>().onPickUp += ResultPickUP;
		}


		private void ResultPickUP()
		{
			Spill();
			_ingredient.GetComponent<NetPickUp>().onPickUp -= ResultPickUP;
			_ingredient = null;
		}

	}
}
