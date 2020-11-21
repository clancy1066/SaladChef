using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    Order m_order;

    public void Clear()
    {
        OrderDispatcher.FreeOrder(m_order);
        m_order = null;
    }

    public bool OrderExpired()
    {
        // Free these up if no order
        if (m_order == null)
            return true;

        return m_order.Expired();
    }
    public bool OrderFullFilled(uint ingredientMask)
    {
        // Free these up if no order
        if (m_order == null)
            return true;

        return m_order.FullFilled(ingredientMask);
    }
    public void SetOrder(Order order)
    {
        m_order = order;

        if (order==null)
            return;

        order.transform.SetParent(transform);

        order.transform.localPosition = Vector3.zero;

        order.gameObject.SetActive(true);
    }

    public int GetOrderCost()
    {
        if (m_order != null)
            return m_order.m_value;
        
        return 0;
    }

    public  float GetOrderWaittime()
    {
        if (m_order != null)
            return m_order.GetCustomerWaitTime();

        return 0;
    }

    public float GetRemainingWaitTime()
    {
        if (m_order != null)
            return m_order.GetRemainingWaitTime();

        return 0;
    }

    

}
