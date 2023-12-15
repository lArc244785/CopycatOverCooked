using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;
using Unity.Netcode;
using System.Collections;

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

        [SerializeField] private List<GameObject> orderPrefabs = new List<GameObject>();
        private GameObject currentPrefab;

        public bool isOrder;

        public bool isSuccess;

        public int _success;
        public int _fail;

        private static OrderManager _instance;    

        public static OrderManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<OrderManager>();

                    if (_instance == null)
                    {
                        Debug.LogError("OrderManager 인스턴스를 찾을 수 없습니다.");
                    }
                }

                return _instance;
            }
        }

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
            if (isOrder && orderableRecipes != null && orderableRecipes.Count > 0)
            {
                GameObject randomOrderPrefab = orderPrefabs[Random.Range(0, orderPrefabs.Count)];
                Debug.Log("생성중");
                // 생성된 주문이 없을 때만 주문 생성
                if (randomOrderPrefab != null)
                {
                    currentPrefab = randomOrderPrefab;
                    Instantiate(currentPrefab);

                    isOrder = false;
                    Debug.Log("생성 완료");
                }
            }
        }

        Order CreateOrder()
        {
            Order nerOrder = new Order();

            return nerOrder;
        }

    }
}
