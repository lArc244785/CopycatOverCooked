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
            // �������� Ÿ���� �����ͼ� untensilTypeIcon�� ������Ʈ
            utensilTypeIcon.sprite = GetUtensilTypeSprite(element.Info.UtensilType);

            // ��� ingredientIcon�� ��Ȱ��ȭ
            DeactivateAllIngredientIcons();

            // element.amount�� Item��ŭ Ȱ��ȭ��Ű�� ingredientIcon�� ������Ʈ
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
            // �������� Ÿ�Կ� ���� ������ Sprite�� ��ȯ
            // �� �κ��� ���ӿ� �°� ���� ������ �ʿ��մϴ�.
            // ��: switch ���� ����Ͽ� �� Ÿ�Կ� ���� Sprite�� ��ȯ�ϵ��� ����
            return null;
        }
    }
}