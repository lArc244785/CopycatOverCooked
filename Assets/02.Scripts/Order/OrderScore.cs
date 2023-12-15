using CopycatOverCooked.Orders;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrderScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI successScore;
    [SerializeField] private TextMeshProUGUI failScore;

    private void Update()
    {
        successScore.text = "Success: " + OrderManager.Instance._success.ToString();
        successScore.text = "Fail: " + OrderManager.Instance._fail.ToString();
    }
}
