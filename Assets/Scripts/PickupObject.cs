using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PickupObject : MonoBehaviour
{

    public GameObject player;
    public float pickUpRange = 5f;
    public LayerMask mask;

    void Update()
    {
        /*
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
        {
            if (hit.transform.GetComponent<InteractableItem>() != null)
            {
                hit.transform.GetComponent<InteractableItem>().DoOnRaycastHit();
                if (Input.GetMouseButtonDown(0))
                {

                    hit.transform.GetComponent<InteractableItem>().DoOnInteract();

                }
            }
        }
        */

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange, mask) && hit.transform.GetComponent<InteractScript>() != null)
            {
                hit.transform.GetComponent<InteractScript>().DoOnInteract();
            }
            else
            {
                ItemManager.GetInstance().GetItemInfo(ItemManager.GetInstance().selectedItemID);
            }
        }
    }


}
