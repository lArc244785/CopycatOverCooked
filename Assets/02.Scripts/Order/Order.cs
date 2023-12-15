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
        //�Ŵ������� ����� ���� ����
        //�ش� ������ �ʱ�ȭ (�����͵� �ʱ�ȭ)


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
            activeOrders++; // �ֹ��� ������ ������ Ȱ�� �ֹ� �� ����
            currentTime = maxTime;
        }

        private void Update()
        {
            currentTime -= Time.deltaTime;

            // Slider�� value�� ����
            timeSlider.value = currentTime / maxTime;

            if (currentTime <= 0f)
            {  
                OrderManager.Instance.isOrder = true;
                // �ֹ� �ð��� ����Ǹ� ���� ó��
                FailOrder();
            }

        }


        public void FailOrder()
        {
            OrderManager.Instance.isSuccess = false;
            activeOrders--; // �ֹ��� �����ϸ� Ȱ�� �ֹ� �� ����
            OnFailed?.Invoke();
            Destroy(gameObject);
            OrderManager.Instance._fail++;
            Debug.Log("Ÿ�� ����");
        }

        public void DeliverOrder()
        {
            OrderManager.Instance.isSuccess = true;
            activeOrders--; // �ֹ��� �Ϸ�Ǹ� Ȱ�� �ֹ� �� ����
            OnDelivered?.Invoke();
            Destroy(gameObject);
            OrderManager.Instance._success++;
            Debug.Log("����~");
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
