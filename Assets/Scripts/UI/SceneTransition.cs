using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private static SceneTransition instance;

    [SerializeField] private CanvasGroup myUIGroup;

    [SerializeField] private bool fadeIn = false;
    [SerializeField] private bool fadeOut = false;

    //can remove after M2
    private float cutsceneDuration = 15;
    private bool onCutscene = false;

    private string nextScene;

    private string currentSceneName;

    public static SceneTransition GetInstance()
    {
        return instance;
    }

    public void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        //app settings
        int screenW = 1920;
        int screenH = 1080;
        bool isFullscreen = false;


        Screen.SetResolution(screenW, screenH, isFullscreen);

        currentSceneName = SceneManager.GetActiveScene().name;
        HideUI();
    }

    public void Start()
    {
        if(currentSceneName == "MainMenu")
        {
            // Unlock and show cursor for UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public string GetCurrentScene()
    {
        return currentSceneName;
    }

    public void StartCutscene()
    {
        onCutscene = true;
    }


    private void Update()
    {
        if (onCutscene)
        {
            cutsceneDuration -= Time.deltaTime;
            if (cutsceneDuration <= 0)
            {
                onCutscene = false;
                ChangeScene("Main");
            }
        }

        if (fadeIn)
        {

            if (myUIGroup.alpha < 1f)
            {
                myUIGroup.alpha += 2 * Time.deltaTime;
                if (myUIGroup.alpha >= 1f)
                {
                    fadeIn = false;
                }
            }
        }

        if (fadeOut)
        {
            if (myUIGroup.alpha >= 0f)
            {
                myUIGroup.alpha -= 2 * Time.deltaTime;
                if (myUIGroup.alpha == 0f)
                {
                    fadeOut = false;
                }
            }
        }

        if (!fadeIn && !fadeOut)
        {
            if (nextScene != null)
            {
                SceneManager.LoadScene(nextScene);
                nextScene = null;
            }
        }

    }

    public void ChangeScene(string sceneName)
    {
        Debug.Log(sceneName);
        ShowUI();
        nextScene = sceneName;
    }

    public void ShowUI()
    {
        fadeIn = true;
    }

    public void HideUI()
    {
        fadeOut = true;
    }

}