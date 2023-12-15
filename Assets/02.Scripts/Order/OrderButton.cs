using CopycatOverCooked.Orders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderButton : MonoBehaviour
{
    public void SuccessButton()
    {
        OrderManager.Instance.isSuccess = true;
    }

    public void FailButton()
    {
        OrderManager.Instance.isSuccess = false;
    }
}
