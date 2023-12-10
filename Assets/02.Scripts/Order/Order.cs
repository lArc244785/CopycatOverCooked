// Order.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;

namespace CopycatOverCooked.Orders
{
    public class Order : MonoBehaviour
    {
        public event System.Action OnFailed;
        public event System.Action OnDelivered;

        private RecipeElementInfo recipe;
        private float waitingTime;
        private bool isCountingDown;

        public void InitializeOrder(RecipeElementInfo recipe, float waitingTime, float startTime)
        {
            this.recipe = recipe;
            this.waitingTime = waitingTime;
            StartCoroutine(CountDownWaitingTimeRoutine(startTime));
        }

        private IEnumerator CountDownWaitingTimeRoutine(float startTime)
        {
            float elapsedTime = 0f;
            while (elapsedTime < startTime)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            while (waitingTime > 0f)
            {
                yield return null;
                waitingTime -= Time.deltaTime;
            }

            waitingTime = 0f;

            // 주문 시간이 종료되면 실패 처리
            FailOrder();
        }

        private void FailOrder()
        {
            OnFailed?.Invoke();
            Destroy(gameObject); // 주문이 실패하면 UI를 파괴
        }

        public void DeliverOrder()
        {
            OnDelivered?.Invoke();
            Destroy(gameObject); // 주문이 완료되면 UI를 파괴
        }
    }
}
