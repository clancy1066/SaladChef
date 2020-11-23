using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    List<Ingredient> m_ingredients = new List<Ingredient>();

    public  int     m_value;
    uint            m_recipeMask;
    float           m_customerWaitTime;
    float           m_customerWaitTimeRemaining;
    bool            m_fulfilled;

    TextMesh m_text;

    // Offsets for placing ingredients
    static Vector3[] m_ingredientOffsets = { Vector3.right, -Vector3.right, Vector3.up };

    // Start is called before the first frame update
    void Start()
    {
        m_text      = GetComponentInChildren<TextMesh>();

        m_fulfilled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (m_text)
        {
            string newText = ("");

            /*
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT1)) == Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT1)) newText += "1_";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT2)) == Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT2)) newText += "2_";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT3)) == Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT3)) newText += "3_";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT4)) == Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT4)) newText += "4_"; ;
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT5)) == Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT5)) newText += "5_";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT6)) == Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT6)) newText += "6_"; ;
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT7)) == Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT7)) newText += "7_";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT8)) == Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT8)) newText += "8_"; ;
            */
            newText += ("Value: " + m_value);

            int minutes = (int)((m_customerWaitTimeRemaining + 0.5f) / 60.0f);
            int seconds = (int)m_customerWaitTimeRemaining % 60;

            newText += ("\nTime:\t"+ minutes.ToString() + ":" + seconds.ToString());

            m_text.text = newText;
        }

        if (m_customerWaitTimeRemaining > 0)
            m_customerWaitTimeRemaining -= Time.deltaTime;

    }

    public void Go()
    {
        m_fulfilled = false;

        m_customerWaitTimeRemaining = m_customerWaitTime;

        Main.AUDIO_Info();
    }

    public void Clear()
    {
        m_recipeMask = 0;

        foreach (Ingredient ingredient in m_ingredients)
            Ingredient.Release(ingredient);

        m_customerWaitTimeRemaining = m_customerWaitTime;
        m_ingredients.Clear();
    }
    public float GetCustomerWaitTime()
    {
        return m_customerWaitTimeRemaining;
    }

    public float GetRemainingWaitTime()
    {
        return m_customerWaitTime;
    }

    public bool Expired()
    {
        return (m_customerWaitTimeRemaining<=0);
    }
    public bool IsFullFilled()
    {
        return m_fulfilled;
    }


    public bool CheckFullFilled(uint ingredientMask)
    {
        m_fulfilled = ((ingredientMask & m_recipeMask) == ingredientMask);

        return m_fulfilled;
    }

    public void SetIngredients(List<Ingredient> ingredients)
    { 
        int offsetCount = 0;

        // Start fresh
        Clear();

        if (ingredients != null)
            foreach (Ingredient ingredient in ingredients)
            {
                if (ingredient.m_grabbedByPlayer)
                    Debug.Log("You gotta be...");

                Ingredient newIngredient = Ingredient.Grab(ingredient.m_ingredientType);

                if (newIngredient == null)
                    continue;

                if (!m_ingredients.Contains(newIngredient))
                    m_ingredients.Add(newIngredient);

               if (!Ingredient.SanityCheck(newIngredient))
                    Debug.Log("For the love of...");

                Vector3 offset = m_ingredientOffsets[offsetCount % m_ingredientOffsets.Length];

                offsetCount++;

                newIngredient.transform.parent = transform;

                newIngredient.transform.localPosition = Vector3.zero;
                newIngredient.transform.position = transform.position+ 0.25f*offset;
               // newIngredient.transform.rotation = transform.rotation;

                m_recipeMask |= ingredient.m_ingredientMask;
            }
    }

    public void Create(List<Ingredient> ingredients, int value, float customerWaitTime)
    {
        m_recipeMask = 0;
        m_fulfilled = false;

        SetIngredients(ingredients);

        m_value = value;
        m_customerWaitTime = customerWaitTime;
    }
}
