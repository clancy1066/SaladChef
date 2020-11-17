using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField]
    public Transform m_holder;

    public Transform GetHolder()
    {
        if (m_holder == null)
            return transform;

        return m_holder;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check ingredient first
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            SendMessageUpwards("OnFoundIngredient", ingredient);
            return;
        }

        // Check ingredient first
        ChoppingTable choppingTable = other.GetComponent<ChoppingTable>();

        if (choppingTable != null)
        {
            SendMessageUpwards("OnFoundChoppingTable", choppingTable);
            return;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        // Check ingredient first
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            SendMessageUpwards("OnClearIngredient", ingredient);
            return;
        }

        // Check ingredient first
        ChoppingTable choppingTable = other.GetComponent<ChoppingTable>();

        if (choppingTable != null)
        {
            SendMessageUpwards("OnClearChoppingTable", choppingTable);
            return;
        }
    }

    
}
