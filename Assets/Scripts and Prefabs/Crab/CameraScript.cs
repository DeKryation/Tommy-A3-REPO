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
        Physics.Raycast(ray, out hit, 1000, mask);
        Debug.Log(hit.collider.transform.gameObject.name);
        Debug.DrawRay(cam.transform.position, cam.transform.forward * 10, Color.green);
        //if hit, show the interact button and play particle
        if (hit.collider.gameObject.tag == "Interactable")
        {
            Debug.Log("interactable hit with raycast.");
            popup.SetActive(true);
            test.DoOnRaycastHit();
            
        }
        else
        {
            popup.SetActive(false);
        }
    }

   
}
