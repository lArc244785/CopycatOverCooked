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

        public void Setup(uint ingredientType)
        {
            if (ingredientType > 0)
            {
                gameObject.SetActive(true);
                this.ingredientType = (IngredientType)ingredientType;
                image.sprite = IngredientSpriteDB.instance.GetSprite(this.ingredientType);

                using (IEnumerator<Image> e1 = igredientIcons.GetEnumerator())
                using (IEnumerator<RecipeElementInfo> e2 = RecipeBook.instance[this.ingredientType].elements.GetEnumerator())
                {
                    while (e1.MoveNext() && e2.MoveNext())
                    {
                        e1.Current.sprite = IngredientSpriteDB.instance.GetSprite(e2.Current.resource);
                    }
                }
            }
            else if (ingredientType < 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}