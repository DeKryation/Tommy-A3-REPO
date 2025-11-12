using UnityEngine;
using UnityEngine.Events;

public class keypad2 : MonoBehaviour
{
    public string password = "1234";
    private string userInput = " ";

    public AudioClip click;
    public AudioClip open;
    public AudioClip no;
    AudioSource source;

    public UnityEvent OnEntryAllowed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        userInput = " ";
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void ButtonClicked(string number)
    {
        source.Play(); // click sound
        userInput = number;
        if (userInput.Length >= 4)
        {
            //check pw
            if (userInput == password)
            {
                source.Play(); //open door sound
            }

            else
            {
                userInput = " ";
                source.Play(); //play no 
            }
        }
    }
}
