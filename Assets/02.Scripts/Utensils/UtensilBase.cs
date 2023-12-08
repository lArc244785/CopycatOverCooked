using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CopycatOverCooked.Datas;
using TMPro.EditorUtilities;
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

    public abstract class UtensilBase : MonoBehaviour
    {      
        [SerializeField] protected UtensilType type;
        [SerializeField] private int _slotCount;
        protected List<IngredientType> slots;
        [SerializeField] private List<RecipeElementInfo> recipeList;
        protected RecipeElementInfo progressRecipe;
        protected ProgressType currentProgress;
        protected float cookProgress;

		public event Action<IngredientType[]> onChangeSlot;
		public event Action<ProgressType, float> onUpdateProgress;

		protected abstract bool CanCooking();
        protected abstract bool CanGrabable();

        protected virtual void Awake()
        {
            recipeList = new List<RecipeElementInfo>();
            recipeList.Capacity = _slotCount;
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
                return true;
            }
            else if(progressRecipe != null && progressRecipe.resource == resource)
            {
                slots.Add(resource);
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


        protected void SurcessProgress()
        {
            for(int i = 0; i < slots.Count; i++)
            {
                slots[i] = progressRecipe.result;
            }
			currentProgress = ProgressType.Sucess;
		}


    }
}
