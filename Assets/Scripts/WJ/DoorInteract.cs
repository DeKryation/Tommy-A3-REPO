using UnityEngine;

public class DoorInteract : InteractScript
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool canClose = true;
    
    [Header("Hover Text")]
    [SerializeField] private string closedHoverText = "Press [LMB] to open door";
    [SerializeField] private string openHoverText = "Press [LMB] to close door";
    
    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    
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
    
    // Public method for hover text
    public string GetHoverText()
    {
        if (isMoving)
            return "";
        
        return isOpen ? openHoverText : closedHoverText;
    }
    
    private void OpenDoor()
    {
        isOpen = true;
        isMoving = true;
        
        if (openSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClip(openSound);
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
