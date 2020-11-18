using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingTable : MonoBehaviour
{
   // Placment
    public Transform    m_playerPos;
    public Transform     m_ingredientPos;

    public Plate        m_currentPlate;

    List<Ingredient>    m_currentIngredients;
    float               m_chopTimer;
    bool                m_doRun = false;

    void Start()
    {
        m_currentIngredients = new List<Ingredient>();
        m_currentPlate = GetComponentInChildren<Plate>();
    }

    public void Run()
    {
        m_doRun = true;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        if (ingredient == null)
            return;

        if (!m_currentIngredients.Contains(ingredient))
            m_currentIngredients.Add(ingredient);

        if (m_ingredientPos != null)
            ingredient.transform.position = m_ingredientPos.position;
        else
            ingredient.transform.position = transform.position;

        m_chopTimer += (ingredient != null ? ingredient.m_choppingTime:0.0f);
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
                if (m_currentPlate)
                {
                    m_currentPlate.AddIngredients(m_currentIngredients);
                    
                    if (Waiter.SubmitPlate(m_currentPlate))
                        Debug.Log("Order Complete");
                    else
                        Debug.Log("Order Failed");

                    m_currentPlate.Clear();
                    m_currentIngredients.Clear();
                }

                m_doRun = false;
            }
        }
    }

    public void OnClick()
    {
        Debug.Log("OnClick");
    }

}
