using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using CopycatOverCooked.Datas;
using System.Collections.Generic;
using System;

namespace CopycatOverCooked.Orders
{
    public class OrderUI : MonoBehaviour
    {
        public Slot slot;
        private List<Slot> slots;
        [SerializeField] private Transform content;

        private void Start()
        {
            OrderManager.instance._orderStates.OnListChanged += OnOrderListChanged;
            slots = new List<Slot>();
            for (int i = 0; i < 20; i++)
            {
                slot = Instantiate(slot, content);
                slot.gameObject.SetActive(false);
                slots.Add(slot);
            }
        }

        private void OnDestroy()
        {
            if(OrderManager.instance != null)
                OrderManager.instance._orderStates.OnListChanged -= OnOrderListChanged;
        }

        private void OnOrderListChanged(NetworkListEvent<OrderState> changeEvent)
        {
            Debug.Log(changeEvent.Index);
            if (changeEvent.Index < 0 || changeEvent.Index >= slots.Count)
                return;

            slots[changeEvent.Index].Setup(changeEvent.Value.ingredientType);

            
        }
    }
}
