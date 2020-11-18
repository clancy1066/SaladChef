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

        order.transform.position = transform.position;
    }
}
