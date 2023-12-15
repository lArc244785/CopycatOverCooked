// Order.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;
using Unity.Netcode;
using UnityEngine.UI;
using System;

namespace CopycatOverCooked.Orders
{
    //ui
    public class Order : NetworkBehaviour
    {
        //매니저에서 사용할 오더 선정
        //해당 오더로 초기화 (데이터들 초기화)


        [SerializeField] private Recipe _recipe;
        //[SerializeField] private GameObject _recipePrefab;

        private NetworkVariable<float> currentTime = new NetworkVariable<float>();
        private NetworkVariable<float> maxTime = new NetworkVariable<float>();
        private NetworkVariable<IngredientType> result = new NetworkVariable<IngredientType>();
        [SerializeField] private Slider timeSlider;

        public event System.Action OnFailed;
        public event System.Action OnDelivered;

        public static int activeOrders = 0;
        

        private void Start()
        {
            activeOrders++; // 주문이 생성될 때마다 활성 주문 수 증가
            currentTime = maxTime;
        }

        private void Update()
        {
            currentTime -= Time.deltaTime;

            // Slider의 value를 갱신
            timeSlider.value = currentTime / maxTime;

            if (currentTime <= 0f)
            {  
                OrderManager.Instance.isOrder = true;
                // 주문 시간이 종료되면 실패 처리
                FailOrder();
            }

        }


        public void FailOrder()
        {
            OrderManager.Instance.isSuccess = false;
            activeOrders--; // 주문이 실패하면 활성 주문 수 감소
            OnFailed?.Invoke();
            Destroy(gameObject);
            OrderManager.Instance._fail++;
            Debug.Log("타임 오버");
        }

        public void DeliverOrder()
        {
            OrderManager.Instance.isSuccess = true;
            activeOrders--; // 주문이 완료되면 활성 주문 수 감소
            OnDelivered?.Invoke();
            Destroy(gameObject);
            OrderManager.Instance._success++;
            Debug.Log("성공~");
        }

        public void InitOrder(Recipe order)
        {
            if (IsServer == false)
                return;

            currentTime.Value = 0.0f;
            maxTime.Value = 10.0f;
            result.Value = order.result;

        }
    }
}
