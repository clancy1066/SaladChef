using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderDispatcher : MonoBehaviour
{
    static List<Customer> sm_freeCustomers      = new List<Customer>();
    static List<Customer> sm_leavingCustomers   = new List<Customer>();
    static List<Customer> sm_waitingCustomers   = new List<Customer>();
    static List<Order> sm_freeOrders            = new List<Order>();
    static List<Ingredient> sm_ingredients      = new List<Ingredient>();

    List<Ingredient> m_ingredientsTMP           = new List<Ingredient>();

    // Set this in the inspector
    public Customer[] m_allCustomers;

    static Order sm_orderTemplate;
    Order   m_orderTemplate
    {
        get => sm_orderTemplate;
        set { sm_orderTemplate = value;}
    }

    float       m_nextOrderTime     = 3.0f;
    const int   cm_MAX_INGREDIENTS  = 3;

    // Start is called before the first frame update
    void Start()
    {
        m_orderTemplate = GetComponentInChildren<Order>();

        if (m_orderTemplate != null)
            m_orderTemplate.gameObject.SetActive(false);

        // Grab all the customer spots
        m_allCustomers = GetComponentsInChildren<Customer>();

        if (m_allCustomers != null)
            foreach (Customer customer in m_allCustomers)
                sm_freeCustomers.Add(customer);
    }


    void GatherIngredients()
    {
        if (sm_ingredients.Count > 0)
            return;

        // Release the last batch
        foreach (Ingredient ingredient in sm_ingredients)
            Ingredient.Release(ingredient);

        sm_ingredients.Clear();

        // Used to create new orders
        for (INGREDIENT_TYPE ingredientType = INGREDIENT_TYPE.INGREDIENT1; ingredientType < INGREDIENT_TYPE.INGREDIENT_MAX; ingredientType++)
        {
            Ingredient newIngredient = Ingredient.Grab(ingredientType);

            if (newIngredient != null)
            {
                sm_ingredients.Add(newIngredient);

                newIngredient.gameObject.SetActive(false);

                if (newIngredient.m_grabbedByPlayer)
                    Debug.Log("Huhu?");
            }
        }
    }

    // *********************************************
    // Orders
    // *********************************************

    Order CreateOrder(float duration,int value, List<Ingredient> ingredients)
    {
        if (m_orderTemplate == null || ingredients==null || ingredients.Count<1)
            return null;

        Order order = AllocOrder();

        order.Create(ingredients, value, duration);

        return order;
    }

    static public Order AllocOrder()
    {
        Order order = null;

        if (sm_freeOrders.Count > 0)
        {
            order = sm_freeOrders[0];
            sm_freeOrders.RemoveAt(0);
        }
        else
            order = Instantiate(sm_orderTemplate);

        order.gameObject.SetActive(true);

        return order;
    }


    static public void FreeOrder(Order order)
    {
        if (order == null)
            return;

        if (!sm_freeOrders.Contains(order))
            sm_freeOrders.Add(order);

        order.Clear();

        order.gameObject.SetActive(false);
    }

    // *******************************************
    // Customer methods
    // *******************************************
    static public void UpdateCustomers()
    {
        foreach (Customer customer in sm_waitingCustomers)
        {
            bool doFree = customer.OrderExpired()||customer.IsSatisfied();
            
            if (customer.OrderExpired())
                Waiter.OrderFailed(customer);
                
             if (doFree)
                FreeCustomerDeferred(customer);
         }
          

        RemoveCustomers();
    }

    static void AddCustomer(Customer customer)
    {
        if (customer != null)
        {
            customer.gameObject.SetActive(true);

            if (!sm_waitingCustomers.Contains(customer))
                sm_waitingCustomers.Add(customer);
        }
    }

    static void FreeCustomerDeferred(Customer customer)
    {
        if (!sm_leavingCustomers.Contains(customer))
            sm_leavingCustomers.Add(customer);
    }

    static void RemoveCustomers()
    {
        foreach (Customer customer in sm_leavingCustomers)
            FreeCustomer(customer);

        sm_leavingCustomers.Clear();
    }

    static public Customer AllocCustomer()
    {
        Customer customer = null;

        if (sm_freeCustomers.Count > 0)
        {
            customer = sm_freeCustomers[0];
            sm_freeCustomers.RemoveAt(0);
        }
        
        AddCustomer(customer);
        
        return customer;
    }


    static public void FreeCustomer(Customer customer)
    {
        if (customer == null)
            return;

        if (!sm_freeCustomers.Contains(customer))
            sm_freeCustomers.Add(customer);

        Order order = customer.GetOrder();

        FreeOrder(order);

        customer.gameObject.SetActive(false);
    }


    void ConstructRandomIngredientList()
    {
        m_ingredientsTMP.Clear();

        if (sm_ingredients.Count < 1)
            return;

        // Ugly but works
        foreach (Ingredient ingredient in sm_ingredients)
            m_ingredientsTMP.Add(ingredient);

        while (m_ingredientsTMP.Count> cm_MAX_INGREDIENTS)
        {
            int index = Random.Range(0, 100) % m_ingredientsTMP.Count;

            m_ingredientsTMP.RemoveAt(index);
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Check outstanding orders
        UpdateCustomers();

        m_nextOrderTime -= Time.deltaTime;

        if (m_nextOrderTime < 0)
        {
            Customer customer = AllocCustomer();

            if (customer!=null)
            { 
                // In case new ones were added - SHould not aloways call this
                GatherIngredients();

                ConstructRandomIngredientList();

                Order newOrder = CreateOrder(Random.Range(30.0f,120.0f), Random.Range(1,10), m_ingredientsTMP);

                customer.SetOrder(newOrder);

                if (!Waiter.AddCustomer(customer))
                {
                    FreeCustomer(customer);
                    FreeOrder(newOrder);
                }
                else
                    newOrder.Go();
            }

            m_nextOrderTime = Random.Range(5.0f, 10.0f);

     
        }
    }
}
