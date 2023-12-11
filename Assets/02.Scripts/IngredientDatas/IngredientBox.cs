using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Datas
{
    public class IngredientBox : NetworkBehaviour
    {
        public GameObject ingredientNetworkPrefab;
        public float unboxingCooldown = 2.0f; // ��ٿ� �ð��� �����մϴ�. ���÷� 5�ʸ� ����մϴ�.
        private double lastUnboxingTime; // ���������� ��Ḧ ������ �ð��� ����մϴ�.
        public IngredientType ingredientType; // �� ���ڿ� ������ ��� Ÿ��

        [ServerRpc]
        public void UnBoxingServerRpc()
        {
            // ���� �ð��� ������ unboxing �ð� + ��ٿ� �ð����� Ŭ ��쿡�� �����մϴ�.
            if (NetworkManager.ServerTime.Time - lastUnboxingTime >= unboxingCooldown)
            {
                GameObject ingredientObject = Instantiate(ingredientNetworkPrefab, transform.position, Quaternion.identity);
                ingredientObject.GetComponent<NetworkObject>().Spawn();

                Ingredient ingredientComponent = ingredientObject.GetComponent<Ingredient>();
                ingredientComponent.type.Value = ingredientType; // NetworkVariable�� ���� ����ȭ�˴ϴ�.

                lastUnboxingTime = NetworkManager.ServerTime.Time; // ������ unboxing �ð��� �����մϴ�.
            }
        }
    }
}