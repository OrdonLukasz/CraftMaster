using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Camera _camera;
    

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 3))
            {
               //picking items
            }
        }
    }
}
