using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{

    List<Ingredient> m_ingredients;    
    
    // Start is called before the first frame update
    void Start()
    {
        m_ingredients = new List<Ingredient>();
    }

    public void AddIngredientq(List<Ingredient> ingredients)
    {
        if (ingredients == null)
            return;

        foreach (Ingredient ingredient in ingredients)
        {
            ingredient.transform.position = transform.position;

            if (!m_ingredients.Contains(ingredient))
                m_ingredients.Add(ingredient);
        }
    }
}
