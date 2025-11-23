using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class keypad2 : MonoBehaviour
{

    [SerializeField] private UnityEvent onAccessYAY;
    public UnityEvent OnAccessYAY => onAccessYAY;

    public float doorOpenTime;

    public string password = "425";
    private string userInput = string.Empty;

    [SerializeField] private TMP_Text keypadDisplayText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        userInput = string.Empty;
    }

    // Update is called once per frame
    public void ButtonClicked(string number)
    {
        //play keypad click
        if(number == "enter")
        {
            StartCoroutine(Check());
        }
        else
        {
            userInput += number;
            keypadDisplayText.text = userInput;
        }
    }       

    public IEnumerator Check()
    {
        //check pw
        if (userInput == password)
        {
            keypadDisplayText.text = "GRANTED";
            onAccessYAY?.Invoke();
            //play granted
            yield return new WaitForSeconds(doorOpenTime);
        }

        else
        {
            keypadDisplayText.text = "DENIED";
            //play denied
            yield return new WaitForSeconds(1.5f);
            keypadDisplayText.text = string.Empty;
            userInput = string.Empty;
        }
    }
}
