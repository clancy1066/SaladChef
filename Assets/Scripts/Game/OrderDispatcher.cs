using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderDispatcher : MonoBehaviour
{
    static List<Order> sm_freeOrders = new List<Order>();

    static List<Ingredient> sm_ingredients = new List<Ingredient>();

    List<Ingredient> m_ingredientsTMP = new List<Ingredient>();

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
            }
        }
    }

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
        m_nextOrderTime -= Time.deltaTime;

        if (m_nextOrderTime < 0)
        {
            // In case new ones were added - SHould not aloways call this
            if (Waiter.HasAvaliableSeating())
            {
                GatherIngredients();

                ConstructRandomIngredientList();

                Order newOrder = CreateOrder(Random.Range(3.0f,12.0f), Random.Range(1,10), m_ingredientsTMP);

                if (!Waiter.AddOrder(newOrder))
                {
                    FreeOrder(newOrder);
                }
                else
                    newOrder.Go();
            }

            m_nextOrderTime = Random.Range(5.0f, 10.0f);

     
        }
    }
}
