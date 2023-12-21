using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using CopycatOverCooked.Datas;
using System.Collections.Generic;

namespace CopycatOverCooked.Orders
{
    public class OrderUI : NetworkBehaviour
    {
        public Slot slot;
        private Transform content;

        void OnOrderListChanged(IEnumerable<OrderState> orderStates)
        {
            foreach(OrderState state in orderStates)
            {
                slot = Instantiate(slot, content);
                //slot.Setup(orderStates, IngredientType);
            }

            
        }

        private void Start()
        {
            OrderManager.instance._orderStates.OnListChanged += OnOrderListChanged;
        }

    }
}
