using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrabMove : MonoBehaviour
{
    private BoxCollider bc;
    private CapsuleCollider cc;
    private Rigidbody rb;
    public Crab_InputSys crabInput;
    public GameObject interactable;
    public float moveSpeed;
    public float jumpForce;

    InputAction pWASD;
    InputAction pJump;
    InputAction pInteract;
    private void Awake()
    {
        crabInput = new Crab_InputSys();
    }
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        pWASD = crabInput.Player.Move; //WASD to move.
        pWASD.Enable();

        pJump = crabInput.Player.Jump; //space to jump. Crab only.
        pJump.Enable();
        pJump.performed += Jump;

        pInteract = crabInput.Player.Interact; //E to interact.
        pInteract.Enable();
        pInteract.performed += TryInteract;
    }
    private void OnDisable()
    {
        pWASD.Disable();
        pJump.Disable();
        pInteract.Disable();
    }

    void Update()
    {
        Vector2 moveDir = pWASD.ReadValue<Vector2>();
        //Debug.Log("directions: x " + moveDir.x + ", y " + moveDir.y);
        transform.position += new Vector3(moveDir.x, 0, moveDir.y) * moveSpeed * Time.deltaTime;
    }
    private void Jump(InputAction.CallbackContext context)
    {
        //Debug.Log("jump");
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void TryInteract(InputAction.CallbackContext context)
    {
        //Debug.Log("what");
        if (interactable != null)
        {
            interactable.GetComponent<InteractScript>().DoOnInteract();
        }
        else
        {
            Debug.Log("No interactable detected");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Interactable")
        {
            Debug.Log("interactable detected!");
            interactable = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (interactable != null && other.gameObject.tag == "Interactable")
        {
            interactable = null;
        }
    }
}
