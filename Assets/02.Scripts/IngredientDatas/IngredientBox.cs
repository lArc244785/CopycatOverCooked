using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Datas
{
    public class IngredientBox : NetworkBehaviour
    {
        public GameObject ingredientNetworkPrefab;
        public float unboxingCooldown = 2.0f; // 쿨다운 시간을 설정합니다. 예시로 5초를 사용합니다.
        private double lastUnboxingTime; // 마지막으로 재료를 생성한 시간을 기록합니다.
        public IngredientType ingredientType; // 이 상자에 설정된 재료 타입

        [ServerRpc]
        public void UnBoxingServerRpc()
        {
            // 현재 시간이 마지막 unboxing 시간 + 쿨다운 시간보다 클 경우에만 실행합니다.
            if (NetworkManager.ServerTime.Time - lastUnboxingTime >= unboxingCooldown)
            {
                GameObject ingredientObject = Instantiate(ingredientNetworkPrefab, transform.position, Quaternion.identity);
                ingredientObject.GetComponent<NetworkObject>().Spawn();

                Ingredient ingredientComponent = ingredientObject.GetComponent<Ingredient>();
                ingredientComponent.type.Value = ingredientType; // NetworkVariable을 통해 동기화됩니다.

                lastUnboxingTime = NetworkManager.ServerTime.Time; // 마지막 unboxing 시간을 갱신합니다.
            }
        }
    }
}