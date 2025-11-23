using UnityEngine;

public class EndScreen : MonoBehaviour
{
    private float timer = 0;

    private bool end;
    [SerializeField] private CanvasGroup endCanvas;

    void Update()
    {
        if (end)
        { 
            if (endCanvas.alpha < 1f)
            {
                endCanvas.alpha += 2 * Time.deltaTime;
                if (endCanvas.alpha >= 1f)
                {
                    end = false;
                    // Unlock and show cursor for UI
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
    }

    public void SetBool(bool value) { end = value; }
}
