using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    Camera cam;
    public Vector2 mouseDelta;
    public Vector3 transformOffset;
    private float rotSpd = 0.1f;
    public GameObject interactable;
    public LayerMask mask;
    public GameObject popup;
    InputHandler inputHandler;

    void Start()
    {
        // Locks the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = GetComponentInChildren<Camera>();
        inputHandler = GetComponentInParent<InputHandler>();
        cam.transform.position += transformOffset;
    }

    // Update is called once per frame
    void Update()
    {
        mouseDelta = Mouse.current.delta.ReadValue();
        float newY = mouseDelta.y * -1;
        Vector3 rotation = new Vector3(newY * rotSpd, mouseDelta.x * rotSpd , 0);
        transform.eulerAngles += rotation;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        ///Raycast interaction - joycelyn
        CheckInteraction();
    }
    public void UnlockCursor()
    {
        // Releases the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    ///trying to implement Raycast Interaction here -Joycelyn
    void CheckInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        
        
        //if hit, show the interact button and play particle
        if (Physics.Raycast(ray, out hit, 2, mask))
        {
            //Debug.Log(hit.collider.transform.gameObject.name);
            Debug.DrawRay(cam.transform.position, cam.transform.forward , Color.green);
            if (hit.collider.gameObject.tag == "Interactable")
            {
                interactable = hit.transform.gameObject;
                //Debug.Log("interactable hit with raycast.");
                popup.SetActive(true);
                interactable.GetComponent<InteractScript>().DoOnRaycastHit();
            }
        }
        else
        {
            popup.SetActive(false);
            interactable = null;
        }
        //reference inputhandler, set interactable to inputhandler's interactable
        inputHandler.SetInteractable(interactable);
    }
    public float GetLookDir()
    {
        float dir = mouseDelta.x; //neg goes left
        return dir;
    }
   
}
