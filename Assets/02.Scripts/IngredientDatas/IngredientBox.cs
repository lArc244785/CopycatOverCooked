using UnityEngine;

namespace CopycatOverCooked.Datas
{
    public class IngredientBox : MonoBehaviour
    {
        [SerializeField]
        private Ingredient ingredientPrefab; // �ν����Ϳ��� ������ Ingredient ������
        public IngredientType ingredientType; // ���� �ȿ� �ִ� ����� Ÿ��

        // ���ڿ��� ��Ḧ ������ �޼���
        public void UnBoxing()
        {
            // ����� �������� ���ɴϴ�.
            Sprite ingredientIcon = GetIngredientIcon(ingredientType);
            // ��� �������� �ν��Ͻ��� �����ϰ� ��ġ�� �����մϴ�.
            Ingredient newIngredient = Instantiate(ingredientPrefab, transform.position, Quaternion.identity);
            newIngredient.type = ingredientType; // ��� Ÿ���� �����մϴ�.
            newIngredient.icon = ingredientIcon; // ��� �������� �����մϴ�.

            // SpriteRenderer ������Ʈ�� �������� �����մϴ�.
            SpriteRenderer spriteRenderer = newIngredient.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = ingredientIcon;
            }
        }

        // ��� Ÿ�Կ� ���� ������ ��������Ʈ�� �������� �޼���
        private Sprite GetIngredientIcon(IngredientType type)
        {
            // ��� Ÿ�Կ� ���� ������ ��������Ʈ�� �����մϴ�.
            // ��: "Resources/Icons/Onion" ��ο� �ִ� ��������Ʈ ���ҽ��� �ε��մϴ�.
            return Resources.Load<Sprite>("Icons/" + type.ToString());
        }
    }
}