using UnityEngine;

namespace CopycatOverCooked.Datas
{
    // ��� Ŭ���� ����
    [System.Serializable]
    public class Ingredient : MonoBehaviour
    {
        public IngredientType type; // ����� Ÿ��
        public Sprite icon; // ����� ������ �̹���

        // Ingredient ������
        public Ingredient(IngredientType type, Sprite icon)
        {
            this.type = type;
            this.icon = icon;
        }
    }
}