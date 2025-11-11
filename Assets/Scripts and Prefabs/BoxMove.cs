using System.Collections;
using UnityEngine;

public class BoxMove : InteractScript
{
    public Transform point1;
    public Transform point2;
    private void Awake()
    {
        transform.position = point1.position;
    }
    public override void DoOnInteract()
    {
        if (transform.position != point2.position)
        {
            StartCoroutine(MoveObject());
        }
    }

    public override void DoOnRaycastHit()
    {
        Debug.Log("Raycast hit");
    }
    IEnumerator MoveObject()
    {
        float timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, point2.position, timeSinceStarted);

            // If the object has arrived, stop the coroutine
            if (transform.position == point2.position)
            {
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }
    }
}
