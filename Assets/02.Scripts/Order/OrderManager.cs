using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using CopycatOverCooked.Datas;
using Unity.VisualScripting;
using CopycatOverCooked.GamePlay;

namespace CopycatOverCooked.Orders
{
    public class OrderManager : NetworkBehaviour
    {
        public static OrderManager instance;
        public NetworkList<OrderState> _orderStates;
        public StageData stageData;
        private float _timer;


        private void Awake()
        {
            instance = this;
            _orderStates = new NetworkList<OrderState>();
        }

        private void Update()
        {
            if (!IsServer)
                return;


            if (_timer <= 0)
            { 
                Order(stageData.menu[Random.Range(0, stageData.menu.Count)]);
                _timer = stageData.orderPeriod;

            }
            else
            {
               
                    _timer -= Time.deltaTime;

            }

            
        }

        private void Order(IngredientType ingredientType)
        {
            Debug.Log(ingredientType);
            _orderStates.Add(new OrderState((uint)ingredientType, Time.time));
        }

        [ServerRpc(RequireOwnership = false)]
        public void DeliveryServerRpc(IngredientType ingredientType)
        {
            for (int i = 0; i < _orderStates.Count; i++)
            {
                if ((uint)ingredientType == _orderStates[i].ingredientType)
                {
                    Debug.Log($"{ingredientType} ���� �Ϸ�");
                    StageManager.instance.currentScore.Value += 100;
                    _orderStates.RemoveAt(i);
                    Debug.Log($"���� ���� ����Ʈ �� {_orderStates.Count}");
                    return;
                }
            }

            // ���� ���
        }

    }
}
