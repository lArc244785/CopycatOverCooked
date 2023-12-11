using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Datas
{
    public class Ingredient : NetworkBehaviour
    {
        public NetworkVariable<IngredientType> type = new NetworkVariable<IngredientType>();
        public Sprite icon; // 네트워크를 통해 동기화할 필요 없으므로 NetworkVariable로 선언하지 않습니다.

        private SpriteRenderer spriteRenderer; // SpriteRenderer 참조를 캐싱하기 위한 필드.

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                type.OnValueChanged += OnIngredientTypeChanged;
            }
            spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer 컴포넌트를 가져옵니다.
        }

        private void OnIngredientTypeChanged(IngredientType oldType, IngredientType newType)
        {
            // 클라이언트 측에서 새로운 'type'에 해당하는 'icon' 스프라이트를 설정합니다.
            icon = GetIngredientIcon(newType);
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = icon;
            }
        }

        private Sprite GetIngredientIcon(IngredientType type)
        {
            // 'type'에 따라 로컬 리소스에서 스프라이트를 로드합니다.
            return Resources.Load<Sprite>("Icons/" + type.ToString());
        }
    }
}