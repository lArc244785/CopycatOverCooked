using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Datas
{
    public class Ingredient : NetworkBehaviour
    {
        public NetworkVariable<IngredientType> type = new NetworkVariable<IngredientType>();
        public Sprite icon; // ��Ʈ��ũ�� ���� ����ȭ�� �ʿ� �����Ƿ� NetworkVariable�� �������� �ʽ��ϴ�.

        private SpriteRenderer spriteRenderer; // SpriteRenderer ������ ĳ���ϱ� ���� �ʵ�.

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                type.OnValueChanged += OnIngredientTypeChanged;
            }
            spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer ������Ʈ�� �����ɴϴ�.
        }

        private void OnIngredientTypeChanged(IngredientType oldType, IngredientType newType)
        {
            // Ŭ���̾�Ʈ ������ ���ο� 'type'�� �ش��ϴ� 'icon' ��������Ʈ�� �����մϴ�.
            icon = GetIngredientIcon(newType);
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = icon;
            }
        }

        private Sprite GetIngredientIcon(IngredientType type)
        {
            // 'type'�� ���� ���� ���ҽ����� ��������Ʈ�� �ε��մϴ�.
            return Resources.Load<Sprite>("Icons/" + type.ToString());
        }
    }
}