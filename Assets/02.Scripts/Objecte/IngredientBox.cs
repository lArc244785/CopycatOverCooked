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
        public float unboxingCooldown = 2.0f; // ��ٿ� �ð��� �����մϴ�. ���÷� 5�ʸ� ����մϴ�.
        private double lastUnboxingTime; // ���������� ��Ḧ ������ �ð��� ����մϴ�.
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

			// ���� �ð��� ������ unboxing �ð� + ��ٿ� �ð����� Ŭ ��쿡�� �����մϴ�.
			if (NetworkManager.ServerTime.Time - lastUnboxingTime >= unboxingCooldown)
            {
                var ingredientObject = Instantiate(prefab, transform.position, Quaternion.identity);
                ingredientObject.GetComponent<NetworkObject>().Spawn();
                ingredientObject.ingerdientType.Value = spawnType;

                lastUnboxingTime = NetworkManager.ServerTime.Time; // ������ unboxing �ð��� �����մϴ�.
                ingredient = ingredientObject;
                return true;
			}
            return false;
        }
    }
}