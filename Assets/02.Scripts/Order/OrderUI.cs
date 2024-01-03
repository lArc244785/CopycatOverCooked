using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

// ���� �߽�
namespace CopycatOverCooked.Orders
{
    public class OrderUI : MonoBehaviour
    {
        [SerializeField] private Transform content;

        public Slot slot;
        private List<Slot> slots;

        private void Start()
        {    
            OrderManager.instance._orderStates.OnListChanged += OnOrderListChanged;
            slots = new List<Slot>();
            for (int i = 0; i < 30; i++)
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
            Debug.Log($"���� �̺�Ʈ �ε��� {changeEvent.Index}");
            //�ڷᱸ�� ��������

            using (IEnumerator<Slot> e1 = slots.GetEnumerator())
            using (IEnumerator<OrderState> e2 = OrderManager.instance._orderStates.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (e2.MoveNext())
                        e1.Current.Setup(e2.Current.ingredientType);
                    else
                        e1.Current.Setup(0);
                }
            }
        }
    }
}
