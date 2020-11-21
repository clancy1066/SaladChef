using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour
{
  //  static Waiter           _inst;

    static List<Customer>   sm_freeCustomers    = new List<Customer>();
    static List<Customer>   sm_waitingCustomers = new List<Customer>();
    static List<Customer>   sm_leavingCustomers = new List<Customer>();

    // Set this in the inspector
    public Customer[]       m_allCustomers;

    // Used for frustrated customers
    static PLAYER_SCORE m_scorePacket = new PLAYER_SCORE();

    // Start is called before the first frame update
    void Start()
    {
        // Grab all the customer spots
        if (m_allCustomers != null)
            foreach (Customer customer in m_allCustomers)
                sm_freeCustomers.Add(customer);
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    static public bool HasAvaliableSeating()
    {
        return (sm_freeCustomers.Count > 0);
    }

    static Customer GetCustomer()
    {
        Customer retVal = null; ;

        if (sm_freeCustomers.Count>0)
        {
            int index = Random.Range(0, 100) % sm_freeCustomers.Count;

            retVal = sm_freeCustomers[index];
            sm_freeCustomers.RemoveAt(index);

            if (!sm_waitingCustomers.Contains(retVal))
                sm_waitingCustomers.Add(retVal);
        }

        return retVal;
    }
    static public void UpdateCustomers()
    {
        foreach (Customer customer in sm_waitingCustomers)
            if (customer.OrderExpired())
            {
                Main.SendFloater(customer.transform.position, 3.0f, ("You guys suck! "));
                Main.AUDIO_Fail();

                m_scorePacket.Set(PLAYER_ID.ANYONE,-customer.GetOrderCost(),-customer.GetOrderWaittime());

                customer.SendMessageUpwards("OnPlayerScored", m_scorePacket);

                sm_leavingCustomers.Add (customer);

                // Clear this out
                customer.Clear();
            }

        RemoveCustomers();
    }
    static void RemoveCustomers()
    {
        foreach (Customer customer in sm_leavingCustomers)
            FreeCustomer(customer);

        sm_leavingCustomers.Clear();
    }

    static void FreeCustomer(Customer customer)
    {  
        if (sm_waitingCustomers.Contains(customer))
            sm_waitingCustomers.Remove(customer);

        if (!sm_freeCustomers.Contains(customer))
            sm_freeCustomers.Add(customer);

    }

    static public bool  AddOrder (Order newOrder)
    {
        // Bad if somebody jumped the gun
        if (newOrder == null)
            return false;

        Customer newCustomer = GetCustomer();

        if (newCustomer == null)
            return false;

        newCustomer.SetOrder(newOrder);

        return true;
    }
    
    static public int SubmitPlate(PLAYER_ID playerID,uint ingredientMask)
    {
        int retVal = 0;

        foreach (Customer customer in sm_waitingCustomers)
            if (customer.OrderFullFilled(ingredientMask))
            {
                // Add score
                Main.SendFloater(customer.transform.position, 2.0f, ("ORDER COMPLETE! "+ customer.GetOrderCost() + " POINTS"));
                Main.AUDIO_Success();

                retVal =  customer.GetOrderCost();

                // Clear this out
                customer.Clear();

                sm_leavingCustomers.Add(customer);

                break;
            }

        RemoveCustomers();

        return retVal;
    }
}
