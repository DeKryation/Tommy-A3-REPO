using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneName = "Lucas' Door QTE";

    [Header("Collision Settings")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool useTrigger = false; // true = use triggers, false = use collisions

    private void OnCollisionEnter(Collision other)
    {
        if (useTrigger) return; // ignore if using triggers

        if (other.collider.CompareTag(targetTag))
        {
            LoadScene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return; // ignore if using collisions

        if (other.CompareTag(targetTag))
        {
            LoadScene();
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
