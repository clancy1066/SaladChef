using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventHandler : MonoBehaviour
{
    public void Hit(float value)
    {
        SendMessageUpwards("OnIngredientEvent");
    }

    public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("someparameter", false);
    }
}
