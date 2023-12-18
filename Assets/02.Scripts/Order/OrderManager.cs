using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
//using Unity.Netcode.NetworkVariable;

namespace CopycatOverCooked.Orders
{
    public class OrderManager : NetworkBehaviour
    {
        // 현재 진행 중인 주문 목록
        private List<NetworkObject> progressOrders = new List<NetworkObject>();

        // 성공한 주문 목록
        private List<NetworkObject> successOrders = new List<NetworkObject>();

        // 실패한 주문 목록
        private List<NetworkObject> failOrders = new List<NetworkObject>();

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

        [SerializeField] private Order _order;

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
            Order newOrder = CreateOrder();

            // NetworkObject를 생성하고 progressOrders에 추가
            var orderNetworkObject = newOrder.gameObject.GetComponent<NetworkObject>();
            if (orderNetworkObject != null)
            {
                progressOrders.Add(orderNetworkObject);
            }
        }

        [ClientRpc]
        void CreateOrderClientRpc()
        {
            // 클라이언트에서는 아무 것도 하지 않음
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

                    // NetworkObject로 주문 생성
                    var orderGameObject = Instantiate(currentPrefab);
                    var orderNetworkObject = orderGameObject.GetComponent<NetworkObject>();
                    if (orderNetworkObject != null)
                    {
                        // 주문 생성 후 progressOrders에 추가
                        progressOrders.Add(orderNetworkObject);
                    }

                    isOrder = false;
                    Debug.Log("생성 완료");
                }
            }
        }

        void SpawnOrder()
        {
            var order = orderableRecipes[0];
            //_order.InitOrder(order);
        }

        Order CreateOrder()
        {
            Order newOrder = new Order();

            return newOrder;
        }
    }
}
