using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavKeypad { 
public class KeypadRaycast : MonoBehaviour
{
    public Camera cam;
    private void Update()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider.TryGetComponent(out ButtonClick keypadButton))
                {
                    keypadButton.KeypadClicked.Invoke();
                }
            }
        }
    }
}
}