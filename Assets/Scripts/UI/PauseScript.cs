using UnityEngine;

public class PauseScript : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    public static bool isPaused;

    private void Start()
    {
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

    }

    public void Resume()
    {
        isPaused = false;
        pauseCanvas.SetActive(false);

        Time.timeScale = 1.0f;
    }

    public void Pause()
    {
        isPaused = true;
        pauseCanvas.SetActive(true);

        Time.timeScale = 0f;
    }
}
