// OrderManager.cs
using System.Collections.Generic;
using UnityEngine;
using CopycatOverCooked.Datas;

namespace CopycatOverCooked.Orders
{
    public class OrderManager : MonoBehaviour
    {
        public GameObject orderPrefab;
        private Transform ordersParent;

        private List<Order> orders = new List<Order>();
        private List<RecipeElementInfo> orderableRecipes = new List<RecipeElementInfo>();

        void Start()
        {
            ordersParent = GameObject.Find("OrdersParent").transform;
            InvokeRepeating("RandomOrderRegister", 3f, 10f); // 3�� �ĺ��� 10�� �������� �ֹ� ����
        }

        private void RandomOrderRegister()
        {
            RecipeElementInfo randomRecipe = GetRandomRecipe();

            if (randomRecipe != null)
            {
                float startTime = Time.time + 3f; // 3�� �Ŀ� �ֹ� ����
                Order newOrder = CreateOrder(randomRecipe, GetRandomWaitingTime(), startTime);
                orders.Add(newOrder);
            }
        }

        private Order CreateOrder(RecipeElementInfo recipe, float waitingTime, float startTime)
        {
            GameObject orderUI = Instantiate(orderPrefab, ordersParent, false);

            Order orderScript = orderUI.GetComponent<Order>();
            orderScript.InitializeOrder(recipe, waitingTime, startTime);

            orderScript.OnDelivered += () => HandleSuccessOrder(orderScript);
            orderScript.OnFailed += () => HandleFailOrder(orderScript);

            return orderScript;
        }

        private void HandleSuccessOrder(Order order)
        {
            orders.Remove(order);
            // ������ �ֹ� ó��
        }

        private void HandleFailOrder(Order order)
        {
            orders.Remove(order);
            // ������ �ֹ� ó��
        }

        private RecipeElementInfo GetRandomRecipe()
        {
            if (orderableRecipes.Count > 0)
            {
                int randomIndex = Random.Range(0, orderableRecipes.Count);
                return orderableRecipes[randomIndex];
            }

            return null;
        }

        private float GetRandomWaitingTime()
        {
            return Random.Range(5f, 20f);
        }
    }
}
