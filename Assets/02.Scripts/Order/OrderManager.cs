using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
//using Unity.Netcode.NetworkVariable;

namespace CopycatOverCooked.Orders
{
    public class OrderManager : NetworkBehaviour
    {
        // ���� ���� ���� �ֹ� ���
        private List<NetworkObject> progressOrders = new List<NetworkObject>();

        // ������ �ֹ� ���
        private List<NetworkObject> successOrders = new List<NetworkObject>();

        // ������ �ֹ� ���
        private List<NetworkObject> failOrders = new List<NetworkObject>();

        // �ֹ� ������ ������ ���
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
                        Debug.LogError("OrderManager �ν��Ͻ��� ã�� �� �����ϴ�.");
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
            // �������� �ֹ��� ó������
            Order newOrder = CreateOrder();

            // NetworkObject�� �����ϰ� progressOrders�� �߰�
            var orderNetworkObject = newOrder.gameObject.GetComponent<NetworkObject>();
            if (orderNetworkObject != null)
            {
                progressOrders.Add(orderNetworkObject);
            }
        }

        [ClientRpc]
        void CreateOrderClientRpc()
        {
            // Ŭ���̾�Ʈ������ �ƹ� �͵� ���� ����
        }

        void InstantiateOrder()
        {
            if (isOrder && orderableRecipes != null && orderableRecipes.Count > 0)
            {
                GameObject randomOrderPrefab = orderPrefabs[Random.Range(0, orderPrefabs.Count)];
                Debug.Log("������");
                // ������ �ֹ��� ���� ���� �ֹ� ����
                if (randomOrderPrefab != null)
                {
                    currentPrefab = randomOrderPrefab;

                    // NetworkObject�� �ֹ� ����
                    var orderGameObject = Instantiate(currentPrefab);
                    var orderNetworkObject = orderGameObject.GetComponent<NetworkObject>();
                    if (orderNetworkObject != null)
                    {
                        // �ֹ� ���� �� progressOrders�� �߰�
                        progressOrders.Add(orderNetworkObject);
                    }

                    isOrder = false;
                    Debug.Log("���� �Ϸ�");
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
