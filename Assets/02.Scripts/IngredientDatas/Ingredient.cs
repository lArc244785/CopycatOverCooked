using UnityEngine;

namespace CopycatOverCooked.Datas
{
    // 재료 클래스 정의
    [System.Serializable]
    public class Ingredient : MonoBehaviour
    {
        public IngredientType type; // 재료의 타입
        public Sprite icon; // 재료의 아이콘 이미지

        // Ingredient 생성자
        public Ingredient(IngredientType type, Sprite icon)
        {
            this.type = type;
            this.icon = icon;
        }
    }
}