using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;
using Unity.Netcode;
using System.Collections;

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
            if (isOrder && orderableRecipes != null && orderableRecipes.Count > 0)
            {
                GameObject randomOrderPrefab = orderPrefabs[Random.Range(0, orderPrefabs.Count)];
                Debug.Log("������");
                // ������ �ֹ��� ���� ���� �ֹ� ����
                if (randomOrderPrefab != null)
                {
                    currentPrefab = randomOrderPrefab;
                    Instantiate(currentPrefab);

                    isOrder = false;
                    Debug.Log("���� �Ϸ�");
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
