using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;
using Unity.Netcode;

namespace CopycatOverCooked.Orders
{
    public class OrderManager : NetworkBehaviour
    {
        [SerializeField] private GameObject orderPrefab;

        private List<Order> orders = new List<Order>();
        private List<RecipeElementInfo> orderableRecipes = new List<RecipeElementInfo>();

        private void Start()
        {
            orderPrefab = GetComponent<GameObject>();
        }

        private void Update()
        {
            CreateOrder();
        }

        void CreateOrder()
        {
            if (orderPrefab != null)
                return;
            else
            {
                Instantiate(orderPrefab);
            }
        }
    }
}
