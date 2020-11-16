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
           
    }
}
