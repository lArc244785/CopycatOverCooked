using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;
using Unity.Netcode;

namespace CopycatOverCooked.Orders
{
    public class OrderManager : NetworkBehaviour
    {
        // ���� ���� ���� �ֹ� ���
        private List<Order> progressOrders = new List<Order>();

        // ������ �ֹ� ���
        private List<Order> successOrders = new List<Order>();

        // ������ �ֹ� ���
        private List<Order> failOrders = new List<Order>();

        // �ֹ� ������ ������ ���
        [SerializeField] private List<Recipe> orderableRecipes = new List<Recipe>();

        [SerializeField] private GameObject orderPrefab;

        private bool isOrder;

        private void Start()
        {
            isOrder = true;
        }

        private void Update()
        {
            if (IsServer)
                CreateOrderServerRpc();

            InstantiateOrder();
        }

        [ServerRpc(RequireOwnership = false)]
        void CreateOrderServerRpc()
        {
            // �������� �ֹ��� ó������
            Order nerOrder = CreateOrder();

        }

        [ClientRpc]
        void CreateOrderClientRpc()
        {
            
        }

        void InstantiateOrder()
        {
            if (isOrder)
            {
                Instantiate(orderPrefab);
                isOrder = false;
            }

        }

        Order CreateOrder()
        {
            Order nerOrder = new Order();

            return nerOrder;
        }
    }
}
