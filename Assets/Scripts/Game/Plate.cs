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

    public void AddIngredientq(List<Ingredient> ingredients)
    {
        if (ingredients==null)
            return;

       
    }
}
