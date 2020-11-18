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

    // Offsets for placing ingredients
    static Vector3[] m_ingredientOffsets = { Vector3.right, -Vector3.right, Vector3.up };

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

            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT1)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT1)) newText += "1_";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT2)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT2)) newText += "2_";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT3)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT3)) newText += "3_";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT4)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT4)) newText += "4_"; ;
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT5)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT5)) newText += "5_";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT6)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT6)) newText += "6_"; ;
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT7)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT7)) newText += "7_";
            if ((m_recipeMask & Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT8)) ==Ingredient.BitMask(INGREDIENT_TYPE.INGREDIENT8)) newText += "8_"; ;

             newText += (" Value: " + m_value + " Time: " + m_customerWaitTime);

            m_text.text = newText;
        }
    }

    public void Clear()
    {
        m_recipeMask = 0;

        foreach (Ingredient ingredient in m_ingredients)
            Ingredient.Release(ingredient);
    }

     public bool FullFilled(uint ingredientMask)
    {
        return ((ingredientMask & m_recipeMask) == ingredientMask);
    }

    public void SetIngredients(List<Ingredient> ingredients)
    { 
        int offsetCount = 0;

        // Start fresh
        Clear();

        if (ingredients != null)
            foreach (Ingredient ingredient in ingredients)
            {
                Ingredient newIngredient = Ingredient.Grab(ingredient.m_ingredientType);

                if (newIngredient == null)
                    continue;

                m_ingredients.Add(newIngredient);

                Vector3 offset = m_ingredientOffsets[offsetCount % m_ingredientOffsets.Length];

                offsetCount++;

                newIngredient.transform.parent = transform;

                newIngredient.transform.localPosition = Vector3.zero;
                newIngredient.transform.position = transform.position+ 0.25f*offset;
                newIngredient.transform.rotation = transform.rotation;

                m_recipeMask |= ingredient.m_ingredientMask;
            }
    }

    public void Create(List<Ingredient> ingredients, uint value, float customerWaitTime)
    {
        m_recipeMask = 0;

        SetIngredients(ingredients);

        m_value = value;
        m_customerWaitTime = customerWaitTime;
    }
}
