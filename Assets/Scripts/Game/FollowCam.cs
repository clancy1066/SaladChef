using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    Camera theCamera;
   
    // Start is called before the first frame update
    void Start()
    {
        theCamera = GetComponent<Camera>();

        Debug.Log("CAMERA + " + theCamera);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
