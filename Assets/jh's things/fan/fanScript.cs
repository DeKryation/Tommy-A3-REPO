using System.Collections;
using UnityEngine;

public class fanScript : MonoBehaviour
{
    bool isInHitbox = false;
    public Rigidbody crabRB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isInHitbox && crabRB != null)
        {
            StartCoroutine(BlowPlayer());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.tag == "Player")
        {
            crabRB = other.attachedRigidbody;
            isInHitbox = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.gameObject.tag == "Player")
        {
            crabRB = null;
            isInHitbox = false;
        }
    }

    IEnumerator BlowPlayer()
    {
        Debug.Log("Player is in fan hitbox.");
        yield return new WaitForSeconds(0.2f);
        crabRB.AddForce(new Vector3(0, 0, 10));
    }
}
