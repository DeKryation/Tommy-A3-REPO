using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    Camera cam;
    public Vector2 mouseDelta;
    public Vector3 transformOffset;
    private float rotSpd = 0.1f;
    void Start()
    {
        // Locks the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = GetComponentInChildren<Camera>();
        cam.transform.position += transformOffset;
    }

    // Update is called once per frame
    void Update()
    {
        mouseDelta = Mouse.current.delta.ReadValue();
        float newY = mouseDelta.y * -1;
        Vector3 rotation = new Vector3(newY * rotSpd, mouseDelta.x * rotSpd , 0);
        transform.eulerAngles += rotation;

        ///Raycast interaction - joycelyn
        CheckInteraction();
    }

    public void UnlockCursor()
    {
        // Releases the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public TestInteract test;
    public LayerMask mask;
    public GameObject popup;
    ///trying to implement Raycast Interaction here -Joycelyn
    void CheckInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        //if hit, show the interact button and play particle
        if(Physics.Raycast(ray, out hit, 30, mask))
        {
            popup.SetActive(true);
            test.DoOnRaycastHit();
            
        }
        else
        {
            popup.SetActive(false);
        }
    }

   
}
