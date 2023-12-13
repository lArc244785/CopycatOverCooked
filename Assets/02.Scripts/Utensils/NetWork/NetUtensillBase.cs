using CopycatOverCooked.Datas;
using CopycatOverCooked.Utensils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UIElements;

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
		/*[공유 데이터]
		 *현재 진행
		 *조리 진행 타입
		 *현재 레시피
		 *넣어져 있는 재료
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
		[SerializeField] private TestPlate _plate;

		public event Action<float, float> onChangeProgress;
		public event Action<IEnumerable<IngredientType>> onChangeSlot; 

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			progress.OnValueChanged += OnChangeProgress;
			inputIngredients.OnListChanged += OnChangeSlot;

			if(IsClient)
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

			if ((ProgressType)progressType.Value == ProgressType.Fail)
				return;

			//재료가 아예 없는 경우
			if (inputIngredients.Count == 0 && CanCookableResource((IngredientType)resource, out var foundRecipeIndex))
			{
				progressType.Value = (int)ProgressType.Progressing;
				recipeIndex.Value = foundRecipeIndex;
				inputIngredients.Add(resource);
				progress.Value = 0.0f;
			}
			//재료가 있는 경우
			else if (inputIngredients.Count > 0 && GetCurrentRecipe().resource == (IngredientType)resource)
			{
				inputIngredients.Add(resource);
				progress.Value = 0.0f;
			}
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
			for(int i =0; i < changeEvent.Index; i++)
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
			for(int i = 0; i < slots.Length; i++)
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
			progress.Value = -1.0f;
			progressType.Value = (int)ProgressType.None;
			return spills;
		}

		public bool CanSpillToPlate(TestPlate plate)
		{
			return progressType.Value == (int)ProgressState.Sucess && 
				   plate.inputIngredients.Count < plate.capacity;
		}


		[ServerRpc(RequireOwnership = false)]
		public void SpillToPlateServerRpc()
		{
			if (CanSpillToPlate(_plate) == false)
				return;

			_plate.InputIngredient(Spill());
		}

		[ServerRpc(RequireOwnership = false)]
		public void SpillToTrashServerRpc()
		{
			Spill();
		}
	}
}
