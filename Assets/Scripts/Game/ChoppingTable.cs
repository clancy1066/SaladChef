using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingTable : MonoBehaviour
{
    [SerializeField]
    public Transform m_playerPos;

    [SerializeField]
    public Transform m_ingredientPos;

    Ingredient  m_currentIngredient;
    float       m_chopTimer;

    public void SetIngredient(Ingredient ingredient)
    {
        m_currentIngredient = ingredient;

        if (m_ingredientPos != null)
            m_currentIngredient.transform.position = m_ingredientPos.position;
        else
            m_currentIngredient.transform.position = transform.position;

        m_chopTimer = (ingredient != null ? ingredient.m_choppingTime:0.0f);
    }

    public Transform GetPlayerPos()
    {
        if (m_playerPos != null)
            return m_playerPos;

        return transform;
    }

    public bool Done()
    {
        if (m_currentIngredient != null && m_chopTimer >= 0.0f)
            return false;

        return true;

    }

    // Update is called once per frame
    void Update()
    {
        if (!Done())
        {
            m_chopTimer -= Time.deltaTime;
            return;
        }
    }
}
