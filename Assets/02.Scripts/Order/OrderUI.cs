using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using CopycatOverCooked.Orders;
using CopycatOverCooked.Datas;

namespace CopycatOverCooked.Orders
{
    public class OrderUI : NetworkBehaviour
    {
        [SerializeField] private Image foodTitle;
        [SerializeField] private Slider progress;
        [SerializeField] private RecipeElementUI[] recipeElement;

        // OrderUI에 현재 처리 중인 Order를 저장하는 변수
        private Order currentOrder;

        // OrderUI가 처리 중인 Order 변경 시 발생하는 이벤트
        public event System.Action<Order> OnOrderChanged;

        // OrderUI 초기화
        public void SetupOrder(Order order)
        {
            // 결과 음식의 타입을 가져와서 Sprite를 얻어와 foodTitle을 업데이트
            IngredientType resultType = order.GetResultType();
            Sprite resultSprite = IngredientUISpriteData.instance.GetSprite(resultType);
            foodTitle.sprite = resultSprite;

            // 현재 처리 중인 Order 변경
            currentOrder = order;
            OnOrderChanged?.Invoke(currentOrder);

            // Order의 OnChangeTime에 progress 업데이트를 등록
            order.OnChangeTime += UpdateProgress;

            // 모든 recipeElement를 비활성화
            DeactivateAllRecipeElements();

            // Order의 elementTable의 Item만큼 활성화하면서 SetUpRecipeElement 호출
            int i = 0;
            foreach (var element in order.GetElementTable())
            {
                if (i < recipeElement.Length)
                {
                    SetUpRecipeElement(recipeElement[i], element);
                    i++;
                }
            }
        }

        private void UpdateProgress(float progressValue)
        {
            // progress를 UI에 업데이트하는 로직 추가
            progress.value = progressValue;
        }

        private void DeactivateAllRecipeElements()
        {
            foreach (var elementUI in recipeElement)
            {
                elementUI.gameObject.SetActive(false);
            }
        }

        private void SetUpRecipeElement(RecipeElementUI elementUI, RecipeElement element)
        {
            elementUI.SetupRecipeElement(element);
            elementUI.gameObject.SetActive(true);
        }
    }
}
