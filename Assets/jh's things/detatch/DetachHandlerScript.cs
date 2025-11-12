using UnityEngine;

public class DetachHandlerScript : MonoBehaviour
{
    public GameObject crab;
    public GameObject crabCanvas;
    public GameObject player;
    public GameObject invCanvas;
    public GameObject dialogueCanvas;
    public GameObject pCamHolder;
    public Transform spawnPoint;

    [SerializeField] private bool isPlayer = true;

    private static DetachHandlerScript _instance;
    public void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this);
        else
            _instance = this;
    }
    public static DetachHandlerScript GetInstance()
    {
        return _instance;
    }
    public void DoSwitch()
    {
        if (isPlayer == true)
        {
            isPlayer = false;
            crab.SetActive(true);
            crabCanvas.SetActive(true);
            player.SetActive(false);
            invCanvas.SetActive(false);
            dialogueCanvas.SetActive(false);
            pCamHolder.SetActive(false);
            crab.transform.position = spawnPoint.position;
        }
        else
        {
            isPlayer = true;
            crab.SetActive(false);
            crabCanvas.SetActive(false);
            player.SetActive(true);
            invCanvas.SetActive(true);
            dialogueCanvas.SetActive(true);
            pCamHolder.SetActive(true);
        }
    }
}
