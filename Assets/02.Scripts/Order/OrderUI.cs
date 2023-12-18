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

        // OrderUI�� ���� ó�� ���� Order�� �����ϴ� ����
        private Order currentOrder;

        // OrderUI�� ó�� ���� Order ���� �� �߻��ϴ� �̺�Ʈ
        public event System.Action<Order> OnOrderChanged;

        // OrderUI �ʱ�ȭ
        public void SetupOrder(Order order)
        {
            // ��� ������ Ÿ���� �����ͼ� Sprite�� ���� foodTitle�� ������Ʈ
            IngredientType resultType = order.GetResultType();
            Sprite resultSprite = IngredientUISpriteData.instance.GetSprite(resultType);
            foodTitle.sprite = resultSprite;

            // ���� ó�� ���� Order ����
            currentOrder = order;
            OnOrderChanged?.Invoke(currentOrder);

            // Order�� OnChangeTime�� progress ������Ʈ�� ���
            order.OnChangeTime += UpdateProgress;

            // ��� recipeElement�� ��Ȱ��ȭ
            DeactivateAllRecipeElements();

            // Order�� elementTable�� Item��ŭ Ȱ��ȭ�ϸ鼭 SetUpRecipeElement ȣ��
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
            // progress�� UI�� ������Ʈ�ϴ� ���� �߰�
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
