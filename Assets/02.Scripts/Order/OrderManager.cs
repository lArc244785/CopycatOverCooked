using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using CopycatOverCooked.Datas;

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

        [ServerRpc]
        private void DeliveryServerRpc(IngredientType ingredientType)
        {
            foreach (OrderState state in _orderStates)
            {
                if ((uint)ingredientType == state.ingredientType)
                {
                    

                    return; 
                }  
            }
            // ³ª»Û °á°ú
        }

    }
}
