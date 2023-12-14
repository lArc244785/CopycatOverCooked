using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;
using Unity.Netcode;

namespace CopycatOverCooked.Orders
{
    public class OrderManager : NetworkBehaviour
    {
        // 현재 진행 중인 주문 목록
        private List<Order> progressOrders = new List<Order>();

        // 성공한 주문 목록
        private List<Order> successOrders = new List<Order>();

        // 실패한 주문 목록
        private List<Order> failOrders = new List<Order>();

        // 주문 가능한 레시피 목록
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
            // 서버에서 주문을 처리하자
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
