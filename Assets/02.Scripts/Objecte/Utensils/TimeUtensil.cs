using CopycatOverCooked.Datas;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Utensils
{
	public class TimeUtensil : UtensilBase
	{
		[SerializeField] private LayerMask _detectObjectMask;
		[SerializeField] private string cookableDetectTag;
	
		[SerializeField] private bool _isDetedActiveObject = false;

		public bool isBuring = false;

		public bool canPickUp => true;

		public bool canDrop => true;

		public ulong owner { get => _owner;}

		private ulong _owner;

		protected override bool CanCooking()
		{
			return (currentProgress == ProgressType.Progressing || currentProgress == ProgressType.Sucess) &&
					_isDetedActiveObject; 
		}

		protected override bool CanGrabable()
		{
			return (currentProgress == ProgressType.Progressing && _isDetedActiveObject) == false && 
				    isBuring == false;
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
			UpdateSlot();
			//todo fire

			currentProgress = ProgressType.Fail;
			cookProgress += Time.deltaTime;
			Debug.Log("Utensill Cooking Fail");
			return;
		}

		private void Update()
		{
			if (CanCooking() == false)
				return;

			UpdateProgress();
		}

		private void OnTriggerEnter(Collider other)
		{
			if((1<< other.gameObject.layer & _detectObjectMask) > 0)
			{
				_isDetedActiveObject = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if ((1 << other.gameObject.layer & _detectObjectMask) > 0)
			{
				_isDetedActiveObject = false;
			}
		}

		[ServerRpc(RequireOwnership = false)]
		public void PickUpServerRpc(ServerRpcParams parmas = default)
		{
			_owner = parmas.Receive.SenderClientId;
		}

		public void Drop()
		{
			throw new System.NotImplementedException();
		}

	}
}
