using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    List<Ingredient> m_ingredients = new List<Ingredient>();
    public uint      m_ingredientMask;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void Clear()
    {
        foreach (Ingredient ingredient in m_ingredients)
            Ingredient.Release(ingredient);

        m_ingredients.Clear();
    }

    public void AddIngredients(List<Ingredient> ingredients)
    {
        m_ingredientMask = 0;

        if (ingredients != null)
            foreach (Ingredient ingredient in ingredients)
            {
                Ingredient newIngredient = Ingredient.Grab(ingredient.m_ingredientType);

                if (newIngredient == null)
                    continue;

                m_ingredients.Add(newIngredient);

                Vector3 addUp = Vector3.up;

                addUp *= (float)newIngredient.m_ingredientType;

                newIngredient.transform.parent = transform;

                newIngredient.transform.localPosition = Vector3.zero;
                newIngredient.transform.position = transform.position + addUp;

                m_ingredientMask |= ingredient.m_ingredientMask;
            }


    }
}
