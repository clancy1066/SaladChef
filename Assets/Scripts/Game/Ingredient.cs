using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum INGREDIENT_TYPE
{
    INGREDIENT1
,   INGREDIENT2
,   INGREDIENT3
,   INGREDIENT4
,   INGREDIENT5
,   INGREDIENT6
,   INGREDIENT7
,   INGREDIENT8
};

public class Ingredient : MonoBehaviour
{
    static Dictionary<INGREDIENT_TYPE, List<Ingredient>> m_spawnLists = new Dictionary<INGREDIENT_TYPE, List<Ingredient>>();

    public INGREDIENT_TYPE  m_ingredientType = INGREDIENT_TYPE.INGREDIENT1;
    public float            m_choppingTime;
    Collider[]              m_colliders;

    // Need this to shut off collision
    Rigidbody               m_rb;

    private void Start()
    {
        InstanceInit();
    }

    public void InstanceInit()
    {
        if (!m_spawnLists.ContainsKey(m_ingredientType))
        {
            m_spawnLists[m_ingredientType] = new List<Ingredient>();
            m_spawnLists[m_ingredientType].Add(this);
        }

        m_colliders = GetComponentsInChildren<Collider>();

        m_rb = GetComponentInChildren<Rigidbody>();
    }

    public void ActivateCollision(bool onOrOff)
    {
        if (m_rb != null)
        {
            m_rb.detectCollisions = onOrOff;
       //     m_rb.gameObject.SetActive(onOrOff);
        }
    }
    static public Ingredient Grab(INGREDIENT_TYPE ingredientType) 
    {
        Ingredient retVal = null;

        if (!m_spawnLists.ContainsKey(ingredientType))
            return retVal;

        if (m_spawnLists[ingredientType].Count > 1)
        {
            int index = (m_spawnLists[ingredientType].Count - 1);
      
            retVal = m_spawnLists[ingredientType][index];

            m_spawnLists[ingredientType].RemoveAt(index);
        }
        else
            retVal = Instantiate(m_spawnLists[ingredientType][0]);

        if (retVal != null)
        {
            retVal.InstanceInit();
            retVal.gameObject.SetActive(true);
            retVal.ActivateCollision(false);

            retVal.transform.localScale = m_spawnLists[ingredientType][0].transform.localScale;
        }
        return retVal;
    }

    static public void Release(Ingredient ingredient)
    {
        if (ingredient == null)
            return;

        if (!m_spawnLists.ContainsKey(ingredient.m_ingredientType))
            return;

        if (!m_spawnLists[ingredient.m_ingredientType].Contains(ingredient))
            m_spawnLists[ingredient.m_ingredientType].Add(ingredient);

        // Leave it laying around if wqe don't reach here
        ingredient.gameObject.SetActive(false);

    }
}
