using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    List<Ingredient> m_ingredients = new List<Ingredient>();

    uint    m_value ;
    uint    m_recipeMask;
    float   m_customerWaitTime;

    TextMesh m_text;

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponentInChildren<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_text)
        {
            string newText = ("Order: ");

            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT1)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT1)) newText += ".1._";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT2)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT2)) newText += ".2.";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT3)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT3)) newText += ".3._";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT4)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT4)) newText += ".4."; ;
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT5)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT5)) newText += ".5._";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT6)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT6)) newText += ".6."; ;
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT7)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT7)) newText += ".7._";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT8)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT8)) newText += ".8."; ;

             newText += (" Value: " + m_value + " Time: " + m_customerWaitTime);

            m_text.text = newText;
        }
    }


    public bool FullFilled(uint ingredientMask)
    {
        return ((ingredientMask & m_recipeMask) == ingredientMask);
    }

    public void AddIngredients(List<Ingredient> ingredients)
    {
        m_recipeMask = 0;

        if (ingredients != null)
            foreach (Ingredient ingredient in ingredients)
            {
                Ingredient newIngredient = Ingredient.Grab(ingredient.m_ingredientType);

                if (newIngredient == null)
                    continue;

                m_ingredients.Add(newIngredient);

                m_recipeMask |= ingredient.m_ingredientMask;
            }
    }

    public void Create(List<Ingredient> ingredients, uint value, float customerWaitTime)
    {
        m_recipeMask = 0;

        AddIngredients(ingredients);

        m_value = value;
        m_customerWaitTime = customerWaitTime;
    }
}
