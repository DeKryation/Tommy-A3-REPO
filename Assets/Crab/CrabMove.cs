using UnityEngine;
using UnityEngine.InputSystem;

public class CrabMove : MonoBehaviour
{
    private BoxCollider bc;
    private Rigidbody rb;
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input)
        {
            transform.position += new Vector3(0, 0, 0.1f);
        }
        else if (Input.GetKeyDown("a"))
        {
            transform.position += new Vector3(-0.1f, 0, 0);
        }
        else if (Input.GetKeyDown("d"))
        {
            transform.position += new Vector3(0.1f, 0, 0);
        }
        else if (Input.GetKeyDown("s"))
        {
            transform.position += new Vector3(0, 0, -0.1f);
        }
    }
}
