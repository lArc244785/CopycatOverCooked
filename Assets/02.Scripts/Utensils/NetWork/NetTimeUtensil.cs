using CopycatOverCooked.Datas;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.NetWork.Untesils
{
	public class NetTimeUtensil : NetUtensillBase
	{
		[SerializeField] private LayerMask _detectObjectMask;
		[SerializeField] private string cookableDetectTag;

		[SerializeField] private NetworkVariable<bool> _isDetedActiveObject = new();

		public NetworkVariable<bool> isBuring = new();

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			if (IsServer == false)
				return;

			_isDetedActiveObject.Value = false;
			isBuring.Value = false;
		}


		private void Update()
		{
			if (IsServer == false)
				return;

			if (CanCooking() == false)
				return;

			UpdateProgress();
		}

		public override void UpdateProgress()
		{
			switch (currentProgressState.Value)
			{
				case ProgressState.Progressing:
					currentProgress.Value += Time.deltaTime;
					if (currentProgress.Value >= currentRecipe.cookSucessProgress)
						SurcessProgress();
					break;

				case ProgressState.Sucess:
					currentProgress.Value += Time.deltaTime;
					if (currentProgress.Value >= currentRecipe.cookFailProgress)
						FailProgress();
					break;
			}
		}

		private void FailProgress()
		{
			slots.Clear();
			slots.Add((int)IngredientType.Trash);
			//todo fire

			currentProgressState.Value = ProgressState.Fail;
			Debug.Log("Utensill Cooking Fail");
			return;
		}


		protected override bool CanCooking()
		{
			return (currentProgressState.Value == ProgressState.Progressing ||
				   currentProgressState.Value == ProgressState.Sucess) &&
				   _isDetedActiveObject.Value;
		}

		protected override bool CanGrabable()
		{
			return (currentProgressState.Value == ProgressState.Progressing && 
					_isDetedActiveObject.Value) == false &&
					isBuring.Value == false;
		}


		private void OnTriggerEnter(Collider other)
		{
			if (IsServer == false)
				return;

			if ((1 << other.gameObject.layer & _detectObjectMask) > 0)
			{
				_isDetedActiveObject.Value = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (IsServer == false)
				return;

			if ((1 << other.gameObject.layer & _detectObjectMask) > 0)
			{
				_isDetedActiveObject.Value = false;
			}
		}
	}
}
