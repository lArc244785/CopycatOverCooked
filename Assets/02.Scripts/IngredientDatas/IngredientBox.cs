using CopycatOverCooked.Datas;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace CopycatOverCooked.Object
{
    public class IngredientBox : NetworkBehaviour
    {
        [SerializeField] private IngredientType spawnType;
        [SerializeField] private Ingredient prefab;
        public float unboxingCooldown = 2.0f; // 쿨다운 시간을 설정합니다. 예시로 5초를 사용합니다.
        private double lastUnboxingTime; // 마지막으로 재료를 생성한 시간을 기록합니다.
        [SerializeField] private TextMeshProUGUI m_boxNameText;

		private void Awake()
		{
            m_boxNameText.text = spawnType.ToString();
		}

		public bool TryGetIngredient(out Ingredient ingredient)
        {
            ingredient = null;
            if (IsServer == false)
                return false;

			// 현재 시간이 마지막 unboxing 시간 + 쿨다운 시간보다 클 경우에만 실행합니다.
			if (NetworkManager.ServerTime.Time - lastUnboxingTime >= unboxingCooldown)
            {
                var ingredientObject = Instantiate(prefab, transform.position, Quaternion.identity);
                ingredientObject.GetComponent<NetworkObject>().Spawn();
                ingredientObject.ingerdientType.Value = spawnType;

                lastUnboxingTime = NetworkManager.ServerTime.Time; // 마지막 unboxing 시간을 갱신합니다.
                ingredient = ingredientObject;
                return true;
			}
            return false;
        }
    }
}