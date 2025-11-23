using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneName = "Lucas' Door QTE";

    [Header("Collision Settings")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool useTrigger = false; // true = use triggers, false = use collisions

    // Only true when player is in range of the door/trigger
    private bool canInteract = false;

    private void Update()
    {
        // Only allow interaction when player is in range
        if (!canInteract)
            return;

        // Left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Check if key is selected in inventory
            if (ItemManager.GetInstance() != null &&
                ItemManager.GetInstance().selectedItemID == 4)
            {
                LoadScene();
            }
            else
            {
                Debug.Log("Need key (itemID 4) selected to use this door.");
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (useTrigger) return; // ignore if using triggers

        if (other.collider.CompareTag(targetTag))
        {
            canInteract = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (useTrigger) return;

        if (other.collider.CompareTag(targetTag))
        {
            canInteract = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return; // ignore if using collisions

        if (other.CompareTag(targetTag))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!useTrigger) return;

        if (other.CompareTag(targetTag))
        {
            canInteract = false;
        }
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("SceneTransitionOnCollision: sceneName is empty.");
        }
    }
}
