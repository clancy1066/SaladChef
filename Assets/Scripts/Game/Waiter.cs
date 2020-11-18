using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour
{
    static Waiter           _inst;

    static List<Customer>   sm_freeCustomers    = new List<Customer>();
    static List<Customer>   sm_waitingCustomers = new List<Customer>();
    

    // Set this in the inspector
    public Customer[]       m_allCustomers;

    // Start is called before the first frame update
    void Start()
    {
        _inst = this;

        // Grab all the customer spots
        if (m_allCustomers != null)
            foreach (Customer customer in m_allCustomers)
                sm_freeCustomers.Add(customer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    static Customer GetCustomer()
    {
        Customer retVal = null; ;

        if (sm_freeCustomers.Count>0)
        {
            retVal = sm_freeCustomers[0];
            sm_freeCustomers.RemoveAt(0);

            if (!sm_waitingCustomers.Contains(retVal))
                sm_waitingCustomers.Add(retVal);
        }

        return retVal;
    }

    static void FreeCustomer(Customer customer)
    {  
        if (!sm_waitingCustomers.Contains(customer))
            sm_waitingCustomers.Remove(customer);

        if (!sm_freeCustomers.Contains(customer))
            sm_freeCustomers.Add(customer);

    }

    static public bool  AddOrder (Order newOrder)
    {
        // Bad if somebody jumped the gun
        if (_inst == null)
            return false;

        if (newOrder == null)
            return false;

        Customer newCustomer = GetCustomer();

        if (newCustomer == null)
            return false;

        newCustomer.SetOrder(newOrder);

        return true;
    }

    static public bool SubmitPlate(Plate plate)
    {
        if (plate == null)
            return false;
 
        foreach (Customer customer in sm_waitingCustomers)
            if (customer.OrderFullFilled(plate.m_ingredientMask))
            {
                // Add score

                // Clear this out
                customer.Clear();

                FreeCustomer(customer);

                return true;
            }
        return false;
    }
}
