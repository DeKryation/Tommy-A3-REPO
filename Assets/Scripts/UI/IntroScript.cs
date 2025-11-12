using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    public float delay = 14f;
    public GameObject cont;
    public string nextScene;

    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;

        if (delay < 0)
        {
            cont.SetActive(true);
            if (Input.anyKey)
                SceneTransition.GetInstance().ChangeScene(nextScene);
        }
    }
}
