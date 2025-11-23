using UnityEngine;

public class BreakerInteract : InteractScript
{
    [Header("Breaker Door")]
    [SerializeField] private GameObject breakerDoorObject;
    [Header("Progress Bar")]
    [SerializeField] private ProgBar progBar;
    [Header("Sound")]

    private bool isUnlocked = false;
    private bool isOn = false;

    public void UnlockBreaker()
    {
        isUnlocked = true;
        Debug.Log("Breaker unlocked!");
    }

    public bool IsOn() { return isOn; }

    public string GetHoverText()
    {
        if (!isUnlocked) return "Connect all ports to unlock.";
        if (isOn) return "Breaker is ON";
        return "Press [LMB] to activate breaker";
    }

    public override void DoOnInteract()
    {
        if (!isUnlocked)
        {
            Debug.Log("Locked: Connect all ports first.");
            return;
        }
        if (isOn) return;
        if (breakerDoorObject == null)
        {
            Debug.LogWarning("breakerDoorObject not assigned!");
            return;
        }

        Transform t = breakerDoorObject.transform;
        Vector3 euler = t.localEulerAngles;
        t.localEulerAngles = new Vector3(270f, euler.y, 75f);

        isOn = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(GameSFX.Breaker);

        if (progBar != null)
        {
            progBar.SetFill(100);
            Debug.Log("Progress bar updated via assigned reference.");
        }
        else
        {
            Debug.LogWarning("No progBar reference assigned in BreakerInteract.");
        }

        // Lights
        LightController lightController = FindObjectOfType<LightController>();
        if (lightController != null)
            lightController.LightUpRoom();

        // Message
        InteractObject io = FindObjectOfType<InteractObject>();
        if (io != null)
            io.ShowMessage("Power restored!");

        Debug.Log("Breaker activated: door snapped, lights on, progress advanced.");
    }
}
