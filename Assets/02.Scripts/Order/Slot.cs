using CopycatOverCooked.Datas;
using CopycatOverCooked.Orders;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CopycatOverCooked.Orders
{
    public class Slot : MonoBehaviour
    {
        public IngredientType ingredientType;
        public Image image;
        public List<Image> igredientIcons;
        public Slider slider; 
        [SerializeField] float timer;

        public void Setup(uint ingredientType)
        {
            if (ingredientType > 0)
            {
                gameObject.SetActive(true);
                this.ingredientType = (IngredientType)ingredientType;
                image.sprite = IngredientVisualDataDB.instance.GetSprite(this.ingredientType);

                using (IEnumerator<Image> e1 = igredientIcons.GetEnumerator())
                using (IEnumerator<RecipeElementInfo> e2 = RecipeBook.instance[this.ingredientType].elements.GetEnumerator())
                {
                    while (e1.MoveNext())
                    {
                        if (e2.MoveNext())
                            e1.Current.sprite = IngredientVisualDataDB.instance.GetSprite(e2.Current.result);
                        else
                            e1.Current.sprite = null;
                    }
                }

                slider.value = timer;
            }
            else if (ingredientType < 0)
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (timer > 0.0f)
            {                
                timer -= Time.deltaTime;
                slider.value = timer;
            }
            else if (timer < 0.0f)
            {
                this.gameObject.SetActive(false);
            }


        }
    }
}