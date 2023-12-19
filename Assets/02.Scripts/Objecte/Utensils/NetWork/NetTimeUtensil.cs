using CopycatOverCooked.Datas;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.NetWork.Untesils
{
	public class NetTimeUtensil : NetUtensillBase
	{
		[SerializeField] private string _progressOnTag;
		[SerializeField] private bool _isProgressable = false;

		public bool canPickUp => true;

		public ulong owner => _owner;

		public bool canPointDrop => _owner >= 0;

		private ulong _owner;

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

			switch ((ProgressState)progressType.Value)
			{
				case ProgressState.Progressing:
					if (progress.Value >= GetCurrentRecipe().cookSucessProgress)
						Sucess();
					break;
				case ProgressState.Sucess:
					if (progress.Value >= GetCurrentRecipe().cookFailProgress)
						Fail();
					break;
			}
			progress.Value += Time.deltaTime;
		}

		protected override bool CanCooking()
		{
			return GetCurrentRecipe() != null &&
				(ProgressState)progressType.Value == ProgressState.Progressing ||
				(ProgressState)progressType.Value == ProgressState.Sucess;
		}

		protected override bool CanGrabable()
		{
			return true;
		}

		private void Fail()
		{
			inputIngredients.Clear();
			inputIngredients.Add((int)IngredientType.Trash);
			progressType.Value = (int)ProgressState.Fail;
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

		public override bool TryAddResource(Ingredient resource)
		{
			if(base.TryAddResource(resource)== false)
				return false;

			resource.GetComponent<NetworkObject>().Despawn();
			return true;
		}
	}
}
