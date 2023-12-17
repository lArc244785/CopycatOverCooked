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

	public abstract class NetUtensillBase : NetworkBehaviour, ISpill
	{
		#region NetworkVariable
		/*[���� ������]
		 *���� ����
		 *���� ���� Ÿ��
		 *���� ������
		 *�־��� �ִ� ���
		 */
		protected NetworkVariable<float> progress = new NetworkVariable<float>();
		/// <summary>
		/// ProgressState
		/// </summary>
		protected NetworkVariable<int> progressType = new NetworkVariable<int>((int)ProgressState.None);
		/// <summary>
		/// IngredientType
		/// </summary>
		protected NetworkVariable<int> recipeIndex = new NetworkVariable<int>();
		/// <summary>
		/// IngredientType
		/// </summary>
		protected NetworkList<int> inputIngredients;
		#endregion

		[SerializeField] private int _slotCapcity;
		[SerializeField] private List<RecipeElementInfo> recipeList;
		public event Action<float, float> onChangeProgress;
		public event Action<IEnumerable<IngredientType>> onChangeSlot;

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			progress.OnValueChanged += OnChangeProgress;
			inputIngredients.OnListChanged += OnChangeSlot;

			if (IsClient)
			{
				ConnetionUpdateData();
			}
		}

		protected virtual void Awake()
		{
			inputIngredients = new NetworkList<int>();
		}

		protected abstract bool CanCooking();
		protected abstract bool CanGrabable();
		public abstract void UpdateProgress();

		public RecipeElementInfo GetCurrentRecipe()
		{
			if (recipeIndex.Value < 0)
				return null;

			return recipeList[recipeIndex.Value];
		}


		[ServerRpc(RequireOwnership = false)]
		public void AddResourceServerRpc(int resource)
		{
			if (inputIngredients.Count == _slotCapcity)
				return;

			if ((ProgressState)progressType.Value == ProgressState.Fail)
				return;

			//��ᰡ �ƿ� ���� ���
			if (inputIngredients.Count == 0 && CanCookableResource((IngredientType)resource, out var foundRecipeIndex))
			{
				progressType.Value = (int)ProgressState.Progressing;
				recipeIndex.Value = foundRecipeIndex;
				inputIngredients.Add(resource);
				progress.Value = 0.0f;
			}
			//��ᰡ �ִ� ���
			else if (inputIngredients.Count > 0 && GetCurrentRecipe().resource == (IngredientType)resource)
			{
				inputIngredients.Add(resource);
				progress.Value = 0.0f;
			}
		}

		public virtual bool TryAddResource(Ingredient resource)
		{
			if (inputIngredients.Count == _slotCapcity)
				return false;

			if ((ProgressState)progressType.Value == ProgressState.Fail)
				return false;

			//��ᰡ �ƿ� ���� ���
			if (inputIngredients.Count == 0 && CanCookableResource(resource.type.Value, out var foundRecipeIndex))
			{
				progressType.Value = (int)ProgressState.Progressing;
				recipeIndex.Value = foundRecipeIndex;
				inputIngredients.Add((int)resource.type.Value);
				progress.Value = 0.0f;
				return true;
			}
			//��ᰡ �ִ� ���
			else if (inputIngredients.Count > 0 && GetCurrentRecipe().resource == resource.type.Value)
			{
				inputIngredients.Add((int)resource.type.Value);
				progress.Value = 0.0f;
				return true;
			}
			return false;
		}


		private bool CanCookableResource(IngredientType resource, out int foundRecipeIndex)
		{
			bool isFound = false;
			foundRecipeIndex = -1;

			int i = 0;

			while (isFound == false && i < recipeList.Count)
			{
				if (recipeList[i].resource == resource)
				{
					isFound = true;
					foundRecipeIndex = i;
				}
				i++;
			}
			return isFound;
		}

		private void OnChangeProgress(float prev, float current)
		{
			var currentRecipe = GetCurrentRecipe();
			var sucess = currentRecipe != null ? currentRecipe.cookSucessProgress : 0.0f;

			onChangeProgress?.Invoke(current, sucess);
		}

		private void OnChangeSlot(NetworkListEvent<int> changeEvent)
		{
			if (changeEvent.Type == NetworkListEvent<int>.EventType.Clear)
			{
				onChangeSlot?.Invoke(null);
				return;
			}

			IngredientType[] slots = new IngredientType[changeEvent.Index + 1];
			for (int i = 0; i < changeEvent.Index; i++)
				slots[i] = (IngredientType)inputIngredients[i];
			slots[changeEvent.Index] = (IngredientType)changeEvent.Value;

			onChangeSlot?.Invoke(slots);
		}

		private void ConnetionUpdateData()
		{
			var recipe = GetCurrentRecipe();
			float sucess = recipe != null ? recipe.cookSucessProgress : 0;

			onChangeProgress?.Invoke(progress.Value, sucess);

			IngredientType[] slots = new IngredientType[inputIngredients.Count];
			for (int i = 0; i < inputIngredients.Count; i++)
				slots[i] = (IngredientType)inputIngredients[i];
			onChangeSlot?.Invoke(slots);
		}

		protected IngredientType[] GetSlotToArray()
		{
			if (inputIngredients.Count == 0)
				return null;

			IngredientType[] slots = new IngredientType[inputIngredients.Count];
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i] = (IngredientType)inputIngredients[i];
			}

			return slots;
		}

		public IngredientType[] Spill()
		{
			IngredientType[] spills = GetSlotToArray();
			inputIngredients.Clear();
			recipeIndex.Value = -1;
			progress.Value = 0f;
			progressType.Value = (int)ProgressState.None;
			return spills;
		}

		public bool CanSpillToPlate(Plate plate)
		{
			return progressType.Value == (int)ProgressState.Sucess &&
				   plate.inputIngredients.Count < plate.capacity;
		}

		public void SpillToTrash()
		{
			Spill();
		}


		//[ServerRpc(RequireOwnership = false)]
		//public void SpillToPlateServerRpc()
		//{
		//	if (CanSpillToPlate(_plate) == false)
		//		return;
		//	if (_plate.isDirty.Value)
		//		return;

		//	_plate.AddIngredient(Spill());
		//}

		[ServerRpc(RequireOwnership = false)]
		public void SpillToTrashServerRpc()
		{
			Spill();
		}

		public virtual void Sucess()
		{
			var result = (int)GetCurrentRecipe().result;

			for (int i = 0; i < inputIngredients.Count; i++)
			{
				inputIngredients[i] = result;
			}
			progressType.Value = (int)ProgressState.Sucess;
		}
	}
}
