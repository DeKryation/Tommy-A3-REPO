using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private BoxCollider bc;
    private CapsuleCollider cc;
    private Rigidbody rb;
    public Crab_InputSys crabInput;
    private GameObject interactable;
    public CameraScript camScript;
    public GameObject crabModel;

    public float moveSpeed;
    public float jumpForce;
    public float rotationSpeed;
    RaycastHit rayHit;

    InputAction pWASD;
    InputAction pJump;
    InputAction pLook;
    InputAction pInteract;
    InputAction pSwitch;
    private bool isScuttling = false;
    private void Awake()
    {
        crabInput = new Crab_InputSys();
    }
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        camScript = GetComponentInChildren<CameraScript>();
    }
    private void OnEnable()
    {
        pWASD = crabInput.Player.Move; //WASD to move.
        pWASD.Enable();

        pJump = crabInput.Player.Jump; //space to jump. Crab only.
        pJump.Enable();
        pJump.performed += Jump;

        pLook = crabInput.Player.Look; //mouse to look around
        pLook.Enable();

        pInteract = crabInput.Player.Attack; //LMB to interact.
        pInteract.Enable();
        pInteract.performed += TryInteract;

        pSwitch = crabInput.Player.Crouch; //Left Ctrl to switch between player and crab.
        pSwitch.Enable();
        pSwitch.performed += TryAttach;
    }
    private void OnDisable()
    {
        pWASD.Disable();
        pJump.Disable();
        pInteract.Disable();
    }

    void Update()
    {
        TurnToDir();
        //Vector2 moveDir = pWASD.ReadValue<Vector2>();
        //transform.position += new Vector3(moveDir.x, 0, moveDir.y) * moveSpeed * Time.deltaTime;
        MoveToDir();
    }
    private void Jump(InputAction.CallbackContext context)
    {
        //Debug.Log("jump");
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(GameSFX.Vaulting);            
        }

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

    private void TryAttach(InputAction.CallbackContext context)
    {

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopLoopingSFX(GameSFX.CrabScuttle);

        DetachHandlerScript.GetInstance().DoSwitch();
    }

    public void SetInteractable(GameObject incoming)
    {
        interactable = incoming;
    }

    private void TurnToDir()
    {
        float dir = camScript.GetLookDir();
        crabModel.transform.eulerAngles += new Vector3(0, dir * rotationSpeed, 0);
        crabModel.transform.eulerAngles = new Vector3(0, crabModel.transform.eulerAngles.y, 0);
    }
    private void MoveToDir()
    {
        Vector2 rawMoveDir = pWASD.ReadValue<Vector2>();
        Vector3 moveDir = crabModel.transform.forward * rawMoveDir.y + crabModel.transform.right * rawMoveDir.x;//insert new direction here
        moveDir.y = 0f;
        moveDir.Normalize();
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        bool shouldScuttle = rawMoveDir.sqrMagnitude > 0.01f; // true if moving
        if (shouldScuttle && !isScuttling)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayLoopingSFX(GameSFX.CrabScuttle);
            isScuttling = true;
        }
        else if (!shouldScuttle && isScuttling)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopLoopingSFX(GameSFX.CrabScuttle);
            isScuttling = false;
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Interactable")
    //    {
    //        Debug.Log("interactable detected!");
    //        interactable = other.gameObject;
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (interactable != null && other.gameObject.tag == "Interactable")
    //    {
    //        interactable = null;
    //    }
    //}
}
