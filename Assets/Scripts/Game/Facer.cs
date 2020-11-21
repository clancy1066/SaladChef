using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Facer : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
             Vector3 direction = Camera.main.transform.position - transform.position;

            transform.rotation = Quaternion.LookRotation(direction);
           
        }
    }
}
