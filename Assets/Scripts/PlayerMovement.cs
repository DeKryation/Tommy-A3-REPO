using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float drag;

    public Transform orientation;

    float horizontalInp;
    float verticalInp;

    Vector3 moveDir;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        MyInput();
        SpeedControl();

        rb.linearDamping = drag;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInp = Input.GetAxisRaw("Horizontal");
        verticalInp = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.LeftControl)) // Left Ctrl to switch between crab and player.
        {
            DetachHandlerScript.GetInstance().DoSwitch();
        }
    }

    private void MovePlayer()
    {
        moveDir = orientation.forward * verticalInp + orientation.right * horizontalInp;

        rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector2 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
}
