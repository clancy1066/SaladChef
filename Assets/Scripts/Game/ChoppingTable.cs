using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingTable : MonoBehaviour
{
   // Placment
    public Transform    m_playerPos;
    public Transform    m_ingredientPos;

    List<Ingredient>    m_currentIngredients;
    uint                m_currentIngredientsMask;

    float               m_chopTimer;
    bool                m_doRun = false;

    void Start()
    {
        m_currentIngredients = new List<Ingredient>();
    }

    public void Run()
    {
        m_doRun = true;
    }

    public void AddIngredients(List<Ingredient> ingredients)
    {
        if (ingredients == null || ingredients.Count<1)
            return;

        foreach (Ingredient ingredient in ingredients)
        {
            Ingredient newIngredient = Ingredient.Grab(ingredient.m_ingredientType);

            if (!m_currentIngredients.Contains(newIngredient))
                m_currentIngredients.Add(newIngredient);

            Transform parentTransform = (m_ingredientPos != null ? m_ingredientPos : transform);

            newIngredient.transform.localPosition  = Vector3.zero;
            newIngredient.transform.position       = parentTransform.position;
            newIngredient.transform.rotation       = parentTransform.rotation;

            newIngredient.transform.SetParent(parentTransform);

            m_chopTimer += (newIngredient != null ? newIngredient.m_choppingTime : 0.0f);

            m_currentIngredientsMask |= newIngredient.m_ingredientMask;
        }
    }

    public Transform GetPlayerPos()
    {
        if (m_playerPos != null)
            return m_playerPos;

        return transform;
    }

    public bool Done()
    {
        if (m_currentIngredients.Count>0 && m_chopTimer>=0.0f)
            return false;

        return true;

    }

    public void ClearIngredients()
    {
        m_currentIngredientsMask = 0;

        foreach (Ingredient ingredient in m_currentIngredients)
            Ingredient.Release(ingredient);

        m_currentIngredients.Clear();
    }

    public bool HasIngredients()
    {  
        return (m_currentIngredients.Count>0);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_doRun && !Done())
        {
            m_chopTimer -= Time.deltaTime;

            if (Done())
            {
              if (Waiter.SubmitPlate(m_currentIngredientsMask))
                  Debug.Log("Order Complete");
              else
                  Debug.Log("Order Failed");

                ClearIngredients();
          
                m_doRun = false;
            }
        }
    }

    public void OnClick()
    {
        Debug.Log("OnClick");
    }

}
