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

            // �ֹ� �ð��� ����Ǹ� ���� ó��
            FailOrder();
        }

        private void FailOrder()
        {
            OnFailed?.Invoke();
            Destroy(gameObject); // �ֹ��� �����ϸ� UI�� �ı�
        }

        public void DeliverOrder()
        {
            OnDelivered?.Invoke();
            Destroy(gameObject); // �ֹ��� �Ϸ�Ǹ� UI�� �ı�
        }
    }
}
