using UnityEngine;

public class DoorInteract : InteractScript
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool canClose = true;

    [Header("Lock Settings")]
    [SerializeField] private BreakerInteract breaker;                 
    [SerializeField] private string lockedHoverText = "Power is still off.";
    [SerializeField] private string lockedMessage = "You need to turn the lights on first!";
    
    [Header("Hover Text")]
    [SerializeField] private string closedHoverText = "Press [LMB] to open door";
    [SerializeField] private string openHoverText = "Press [LMB] to close door";

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip lockedSound;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isMoving = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        if (isMoving)
        {
            Quaternion targetRotation = isOpen ? openRotation : closedRotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isMoving = false;
            }
        }
    }

    public override void DoOnInteract()
    {
        if (breaker != null && !breaker.IsOn())
        {
            if (lockedSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClip(lockedSound);
            }

            InteractObject io = FindObjectOfType<InteractObject>();
            if (io != null)
                io.ShowMessage(lockedMessage, true);

            Debug.Log("Door is locked. Lights need to be on.");
            return;
        }

        if (isMoving)
            return;

        if (!isOpen)
        {
            OpenDoor();
        }
        else if (canClose)
        {
            CloseDoor();
        }
    }

    public string GetHoverText()
    {
        if (breaker != null && !breaker.IsOn())
            return lockedHoverText;

        if (isMoving)
            return "";
        return isOpen ? openHoverText : closedHoverText;
    }

    private void OpenDoor()
    {
        isOpen = true;
        isMoving = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(GameSFX.DoorOpen);
        }
        Debug.Log("Door opening");
    }


    private void CloseDoor()
    {
        isOpen = false;
        isMoving = true;
        if (closeSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClip(closeSound);
        }
        Debug.Log("Door closing");
    }
}
