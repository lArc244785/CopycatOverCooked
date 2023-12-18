using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
//using Unity.Netcode.NetworkVariable;

namespace CopycatOverCooked.Orders
{
    public class Order : NetworkBehaviour
    {
        [SerializeField] private Recipe _recipe;
        private Dictionary<RecipeElement, int> _elementTable;

        // NetworkVariable 사용
        private NetworkVariable<float> _currentTime = new NetworkVariable<float>();
        private NetworkVariable<float> _maxTime = new NetworkVariable<float>();

        // 성공 및 실패 이벤트
        public event System.Action OnFailed;
        public event System.Action OnDelivered;

        private void Start()
        {
            // 요리 재료 테이블 초기화
            _elementTable = _recipe.Init();

            // 주문 타이머 시작
            StartCoroutine(OrderTimer());
        }

        private IEnumerator OrderTimer()
        {
            while (_currentTime.Value > 0f)
            {
                yield return null;
                _currentTime.Value -= Time.deltaTime;
            }

            // 시간이 다 되어 주문 실패
            FailOrder();
        }

        private void Update()
        {
            // 레시피를 지속적으로 확인
            CheckRecipe();
        }

        private void CheckRecipe()
        {
            foreach (var element in _elementTable)
            {
                // 각 재료가 충분히 있는지 확인
                if (element.Value <= 0)
                {
                    // 재료가 하나라도 부족하면 주문 실패
                    FailOrder();
                    return;
                }
            }

            // 모든 재료가 충분하면 주문 성공
            DeliverOrder();
        }

        public IEnumerable<RecipeElement> GetElementTable()
        {
            // _elementTable.Values를 IEnumerable<RecipeElement>로 변환
            return _elementTable.Values.Cast<RecipeElement>();
        }

        private void FailOrder()
        {
            OnFailed?.Invoke();
            // 해당 주문 객체를 모든 클라이언트에서 파괴
            NetworkObject.Destroy(gameObject);
        }

        private void DeliverOrder()
        {
            OnDelivered?.Invoke();
            // 해당 주문 객체를 모든 클라이언트에서 파괴
            NetworkObject.Destroy(gameObject);
        }
    }
}