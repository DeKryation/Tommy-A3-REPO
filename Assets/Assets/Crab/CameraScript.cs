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
    }

    public void UnlockCursor()
    {
        // Releases the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
