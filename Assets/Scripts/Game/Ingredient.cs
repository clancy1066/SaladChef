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
,   INGREDIENT_MAX
};

public class Ingredient : MonoBehaviour
{
    static Dictionary<INGREDIENT_TYPE, List<Ingredient>> m_spawnLists = new Dictionary<INGREDIENT_TYPE, List<Ingredient>>();

    public INGREDIENT_TYPE  m_ingredientType = INGREDIENT_TYPE.INGREDIENT1;
    public uint         m_ingredientMask;
    
    public float        m_choppingTime;

    // Appearence
    ColorSetter         m_colorSetter;
    Renderer            m_renderer;

    // Need this to shut off collision
    Rigidbody            m_rb;

    public bool m_grabbedByPlayer   = false;

    // Offsets from root
    public static Vector3[] sm_ingredientOffsets = { Vector3.zero, -Vector3.right, Vector3.right };

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

        // Grab the renderer
        m_renderer = GetComponentInChildren<Renderer>();

        // For setting the color
        m_colorSetter = GetComponentInChildren<ColorSetter>();

        // Used for fast lookup of orders
        m_ingredientMask = (uint)(1 << (int)m_ingredientType);

        m_rb = GetComponentInChildren<Rigidbody>();

        ChangeColor();

        m_grabbedByPlayer = false;
    }

    static public Vector3 GetPlacementOffset(int baseIndex)
    {
        return sm_ingredientOffsets[baseIndex % sm_ingredientOffsets.Length];
    }

    void ChangeColor()
    {
        if (m_colorSetter != null)
            m_colorSetter.SetColor(m_renderer);    
    }

    public void ActivateCollision(bool onOrOff)
    {
        if (m_rb != null)
        {
            m_rb.detectCollisions = onOrOff;
        }
    }

    static public uint BitMask(INGREDIENT_TYPE ingredientType)
    {
        return ((uint)(1<<(int)ingredientType));
    }

    static public Ingredient GetBaseIngredient(Ingredient ingredient)
    { 
        if (!m_spawnLists.ContainsKey(ingredient.m_ingredientType))
            return null;

        return m_spawnLists[ingredient.m_ingredientType][0];
    }

    static public Ingredient Grab(INGREDIENT_TYPE ingredientType) 
    {
        Ingredient retVal = null;

        if (!m_spawnLists.ContainsKey(ingredientType))
            return retVal;

        if (m_spawnLists[ingredientType].Count > 1)
        {
            int index = 1;
      
            retVal = m_spawnLists[ingredientType][index];

            m_spawnLists[ingredientType].RemoveAt(index);
        }
        else
            retVal = Instantiate(m_spawnLists[ingredientType][0]);

        if (retVal.m_grabbedByPlayer)
            Debug.Log("What the?");

        if (retVal != null)
        {
            retVal.InstanceInit();
            retVal.gameObject.SetActive(true);
            retVal.ActivateCollision(false);

            retVal.transform.SetParent(null);
            retVal.transform.localScale = m_spawnLists[ingredientType][0].transform.localScale;
        }

        if (!SanityCheck(retVal))
            Debug.Log("On my ever living...");

        return retVal;
    }

    static public void Release(Ingredient ingredient)
    {
        if (ingredient == null)
            return;

        if (ingredient.m_grabbedByPlayer)
            Debug.Log("Huh?");

        if (!m_spawnLists.ContainsKey(ingredient.m_ingredientType))
            return;

        if (!m_spawnLists[ingredient.m_ingredientType].Contains(ingredient))
            m_spawnLists[ingredient.m_ingredientType].Add(ingredient);
        else
            Debug.Log("Releasing an ingredient already in a list");

        // Leave it laying around if wqe don't reach here
        ingredient.transform.SetParent(null);
        ingredient.gameObject.SetActive(false);

    }

    static public  bool SanityCheck(Ingredient ingredient)
    {
        if (ingredient == null)
            return true;

        return (!m_spawnLists[ingredient.m_ingredientType].Contains(ingredient));
    }
}
