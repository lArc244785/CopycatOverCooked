using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;
using System;

namespace CopycatOverCooked.Utensils
{
    public enum ProgressType 
    {
        None,
        Progressing,
        Sucess,
        Fail,
    }

    public abstract class UtensilBase : MonoBehaviour, ISpill
	{      
        [SerializeField] protected UtensilType type;
        [SerializeField] private int _slotCount;
        protected List<IngredientType> slots;
        [SerializeField] private List<RecipeElementInfo> recipeList;
        protected RecipeElementInfo progressRecipe 
        { 
            get
            {
                return _progressRecipe;
			}
			set
			{
                _progressRecipe = value;
                onChangeRecipe?.Invoke(_progressRecipe);
			}
        }

        private RecipeElementInfo _progressRecipe;

        protected ProgressType currentProgress;
        private float _cookProgress;
        protected float cookProgress
		{
			set
			{
                _cookProgress = value;
                onUpdateProgress?.Invoke(currentProgress, value);
			}
			get
			{
                return _cookProgress;
			}
		}

		public event Action<IngredientType[]> onChangeSlot;
		public event Action<ProgressType, float> onUpdateProgress;
        public event Action<RecipeElementInfo> onChangeRecipe;

		protected abstract bool CanCooking();
        protected abstract bool CanGrabable();

        protected virtual void Awake()
        {
            slots = new();
            slots.Capacity = _slotCount;
            currentProgress = ProgressType.None;
            cookProgress = 0.0f;
        }


        public bool TryAddResource(IngredientType resource)
        {
            if (slots.Count == _slotCount)
                return false;

            if (currentProgress == ProgressType.Fail)
                return false;

            if(slots.Count == 0 && CanCookableResource(resource, out var foundRecipe))
            {
                currentProgress = ProgressType.Progressing;
                progressRecipe = foundRecipe;
                slots.Add(resource);
                UpdateSlot();
                return true;
            }
            else if(progressRecipe != null && progressRecipe.resource == resource)
            {
                slots.Add(resource);
                UpdateSlot();
                cookProgress = 0.0f;
				return true;
            }

            return false;
        }

        private bool CanCookableResource(IngredientType resource, out RecipeElementInfo foundRecipe)
        {
            bool isFound = false;
			foundRecipe = null;

            int i = 0;

			while(isFound == false && i < recipeList.Count)
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

        public abstract void UpdateProgress();

        private IngredientType[] Spill()
        {
            IngredientType[] spills = slots.ToArray();
            slots.Clear();
            progressRecipe = null;
            cookProgress = 0.0f;
            currentProgress = ProgressType.None;
            UpdateSlot();
            return spills;
        }

        public bool TrySpillToPlate(out IngredientType[] spills)
        {
            spills = null;
            if (currentProgress != ProgressType.Sucess)
                return false;

            spills = Spill();
            return true;
        }

        public void SpillToTrash()
        {
            Spill();
		}


        protected virtual void SurcessProgress()
        {
            for(int i = 0; i < slots.Count; i++)
            {
                slots[i] = progressRecipe.result;
            }

            onChangeSlot(slots.ToArray());
			currentProgress = ProgressType.Sucess;
            onUpdateProgress?.Invoke(currentProgress, cookProgress);
            Debug.Log("Utensill Cooking Surcess");
		}

        protected void UpdateSlot()
		{
            onChangeSlot?.Invoke(slots.ToArray());
        }

		IngredientType[] ISpill.Spill()
		{
			throw new NotImplementedException();
		}
	}
}
