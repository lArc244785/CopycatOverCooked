using UnityEngine;

namespace CopycatOverCooked.Datas
{
    public class IngredientBox : MonoBehaviour
    {
        [SerializeField]
        private Ingredient ingredientPrefab; // 인스펙터에서 설정할 Ingredient 프리팹
        public IngredientType ingredientType; // 상자 안에 있는 재료의 타입

        // 상자에서 재료를 꺼내는 메서드
        public void UnBoxing()
        {
            // 재료의 아이콘을 얻어옵니다.
            Sprite ingredientIcon = GetIngredientIcon(ingredientType);
            // 재료 프리팹의 인스턴스를 생성하고 위치를 설정합니다.
            Ingredient newIngredient = Instantiate(ingredientPrefab, transform.position, Quaternion.identity);
            newIngredient.type = ingredientType; // 재료 타입을 설정합니다.
            newIngredient.icon = ingredientIcon; // 재료 아이콘을 설정합니다.

            // SpriteRenderer 컴포넌트에 아이콘을 설정합니다.
            SpriteRenderer spriteRenderer = newIngredient.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = ingredientIcon;
            }
        }

        // 재료 타입에 따른 아이콘 스프라이트를 가져오는 메서드
        private Sprite GetIngredientIcon(IngredientType type)
        {
            // 재료 타입에 따라 적절한 스프라이트를 리턴합니다.
            // 예: "Resources/Icons/Onion" 경로에 있는 스프라이트 리소스를 로드합니다.
            return Resources.Load<Sprite>("Icons/" + type.ToString());
        }
    }
}