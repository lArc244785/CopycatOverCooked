using CopycatOverCooked.Datas;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.NetWork.Untesils
{
	public enum ProgressState
	{
		None,
		Progressing,
		Sucess,
		Fail,
	}

	public abstract class NetUtensillBase : NetworkBehaviour 
	{
		[SerializeField] protected UtensilType type;
		[SerializeField] private int _slotCount;
		[SerializeField] private List<RecipeElementInfo> recipeList;
		protected RecipeElementInfo currentRecipe
		{
			set
			{
				_currentRecipe = value;
				onChangeRecipe?.Invoke(value);
			}
			get
			{
				return _currentRecipe;
			}
		}

		private RecipeElementInfo _currentRecipe;


		#region NetWorkValues
		/// <summary>
		/// 해당 데이터는 꼭 IngredientType으로 변환을 해야됩니다.
		/// </summary>
		protected NetworkList<int> slots = new();
		protected NetworkVariable<float> currentProgress = new();
		protected NetworkVariable<ProgressState> currentProgressState = new();
		public NetworkVariable<IngredientType[]> spills = new();
		#endregion

		public event Action<IEnumerator<int>> onUpdateSlot;
		public event Action<ProgressState, float> onUpdateProgress;
		public event Action<RecipeElementInfo> onChangeRecipe;

		protected abstract bool CanCooking();
		protected abstract bool CanGrabable();
		public abstract void UpdateProgress();

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			if (!IsServer)
				return;

			currentProgress.Value = 0.0f;
			currentRecipe = null;

			currentProgress.OnValueChanged += OnChangeCurrentProgress;
			currentProgressState.OnValueChanged += OnChangeCurrentProgressState;
		}

		private void OnChangeCurrentRecipe(RecipeElementInfo prev, RecipeElementInfo current)
		{
			onChangeRecipe?.Invoke(current);
		}

		private void OnChangeCurrentProgress(float prev, float current)
		{
			onUpdateProgress?.Invoke(currentProgressState.Value, current);
		}

		private void OnChangeCurrentProgressState(ProgressState prev, ProgressState current)
		{
			onUpdateProgress?.Invoke(current, currentProgress.Value);
		}

		private bool TryAddSlot(IngredientType resource)
		{
			if (slots.Count >= _slotCount)
				return false;

			currentProgress.Value = 0.0f;
			slots.Add((int)resource);
			onUpdateSlot?.Invoke(slots.GetEnumerator());
			return true;
		}

		//[ServerRpc(RequireOwnership = false)]
		//public void AddResourceServerRpc(IngredientType resource, ServerRpcParams parasm = default)
		//{
		//	if (currentProgressState.Value == ProgressState.Fail)
		//		return;

		//	if (slots.Value.Count == 0 && CanCookableResource(resource, out var foundRecipe))
		//	{
		//		currentProgressState.Value = ProgressState.Progressing;
		//		currentRecipe.Value = foundRecipe;
		//		TryAddSlot(resource);
		//	}
		//	else if (currentRecipe.Value != null && currentRecipe.Value.resource == resource)
		//	{
		//		TryAddSlot(resource);
		//	}
		//}

		private bool CanCookableResource(IngredientType resource, out RecipeElementInfo foundRecipe)
		{
			bool isFound = false;
			foundRecipe = null;

			int i = 0;

			while (isFound == false && i < recipeList.Count)
			{
				if (recipeList[i].resource == resource)
				{
					isFound = true;
					foundRecipe = recipeList[i];
				}
				i++;
			}
			return isFound;
		}

		private IngredientType[] Spill()
		{
			IngredientType[] spills = new IngredientType[slots.Count];
			for (int i = 0; i < slots.Count; i++)
				spills[i] = (IngredientType)slots[i];

			slots.Clear();
			currentRecipe = null;
			currentProgress.Value = 0.0f;
			currentProgressState.Value = ProgressState.None;
			onUpdateSlot?.Invoke(slots.GetEnumerator());
			return spills;
		}

		[ServerRpc]
		public void SpillToPlateServerRpc()
		{
			if (currentProgressState.Value != ProgressState.Sucess)
				return;

			spills.Value = Spill();
		}

		[ServerRpc]
		public void SpillToTrashServerRpc()
		{
			Spill();
		}


		protected virtual void SurcessProgress()
		{
			for (int i = 0; i < slots.Count; i++)
			{
				slots[i] = (int)currentRecipe.result;
			}

			currentProgressState.Value = ProgressState.Sucess;
			Debug.Log("Utensill Cooking Surcess");
		}

	}
}
