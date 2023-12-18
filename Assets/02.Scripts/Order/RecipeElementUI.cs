using CopycatOverCooked.Datas;
using UnityEngine;
using UnityEngine.UI;

namespace CopycatOverCooked.Orders
{
    public class RecipeElementUI : MonoBehaviour
    {
        [SerializeField] private Image[] ingredientIcons;
        [SerializeField] private Image utensilTypeIcon;

        public void SetupRecipeElement(RecipeElement element)
        {
            // 조리도구 타입을 가져와서 untensilTypeIcon을 업데이트
            utensilTypeIcon.sprite = GetUtensilTypeSprite(element.Info.UtensilType);

            // 모든 ingredientIcon을 비활성화
            DeactivateAllIngredientIcons();

            // element.amount의 Item만큼 활성화시키고 ingredientIcon을 업데이트
            for (int i = 0; i < element.Amount; i++)
            {
                ingredientIcons[i].gameObject.SetActive(true);
                ingredientIcons[i].sprite = IngredientUISpriteData.instance.GetSprite(element.Info.Result);
            }
        }

        private void DeactivateAllIngredientIcons()
        {
            foreach (var icon in ingredientIcons)
            {
                icon.gameObject.SetActive(false);
            }
        }

        private Sprite GetUtensilTypeSprite(UtensilType utensilType)
        {
            // 조리도구 타입에 따라서 적절한 Sprite를 반환
            // 이 부분은 게임에 맞게 실제 구현이 필요합니다.
            // 예: switch 문을 사용하여 각 타입에 대한 Sprite를 반환하도록 수정
            return null;
        }
    }
}