using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour
{
  //  static Waiter           _inst;
    static List<Customer>   sm_waitingCustomers = new List<Customer>();
   
    // Used for frustrated customers
    static PLAYER_SCORE m_scorePacket = new PLAYER_SCORE();


    static void CustomerSuccess(Customer customer)
    {
        Main.AUDIO_Success();

        Main.SendFloater(customer.transform.position, 2.0f, ("ORDER COMPLETE! " + customer.GetOrderCost() + " POINTS"));

        m_scorePacket.Set(customer.GetSubmittingPlayer(), customer.GetOrderCost(), customer.GetRemainingWaitTime());

        FreeCustomer(customer);
    }

    static void CustomerFail(Customer customer)
    {
        Main.AUDIO_Fail();

        Main.SendFloater(customer.transform.position, 2.0f, ("You guys suck!"));

        m_scorePacket.Set(customer.GetSubmittingPlayer(), -customer.GetOrderCost(), -customer.GetOrderWaittime());

        FreeCustomer(customer);
    }

    static public void OrderFailed(Customer customer)
    {
        CustomerFail(customer);

        customer.SendMessageUpwards("OnPlayerScored", m_scorePacket);

        FreeCustomer(customer);
    }

    static public void OrderSatisfied(Customer customer)
    {
        CustomerSuccess(customer);

        customer.SendMessageUpwards("OnPlayerScored", m_scorePacket);

        FreeCustomer(customer);
    }

    static public void FreeCustomer(Customer oldCustomer)
    {
        if (sm_waitingCustomers.Contains(oldCustomer))
            sm_waitingCustomers.Remove(oldCustomer);
    }
 
    static public bool  AddCustomer (Customer newCustomer)
    {
        // Bad if somebody jumped the gun
        if (newCustomer == null)
            return false;

        if (!sm_waitingCustomers.Contains(newCustomer))
            sm_waitingCustomers.Add(newCustomer);

        return true;
    }
    
    static public int SubmitPlate(PLAYER_ID playerID,uint ingredientMask)
    {
        foreach (Customer customer in sm_waitingCustomers)
            if (customer.OrderFullFilled(ingredientMask))
            {
                customer.SetSubmittingPlayer(playerID);
               

                OrderSatisfied(customer);

                return customer.GetOrderCost();
            }
       
 

        return -1;
    }
}
