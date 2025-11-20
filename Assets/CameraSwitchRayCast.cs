using UnityEngine;
using UnityEngine.UI;

public class CameraSwitchRayCast : MonoBehaviour
{
    public Camera main;
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;

    public LayerMask mask;

    private void Start()
    {
        main = Camera.main;
    }

    private void Update()
    {
        RayCast();

        if(Input.GetMouseButton(0))
        {
            RayCastHit();
        }
    }

    void RayCast()
    {
        //get mouse pos
        Vector3 mousePos = Input.mousePosition;
        mousePos = cam1.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(transform.position, mousePos - transform.position, Color.blue);
    }

    void RayCastHit()
    {

        Ray ray = main.ScreenPointToRay(Input.mousePosition); //shoot out the ray
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 150, mask))
        {
            Debug.Log(hit.transform.name);

            if(hit.transform.name == "Button_1")
            {
                cam1.gameObject.SetActive(true);
                cam2.gameObject.SetActive(false);
                cam3.gameObject.SetActive(false);
                //cctv_cam switch sfx here
            }
            else if(hit.transform.name == "Button_2")
            {
                cam2.gameObject.SetActive(true);
                cam1.gameObject.SetActive(false);
                cam3.gameObject.SetActive(false);
                //cctv_cam switch sfx here
            }
            else if(hit.transform.name == "Button_3")
            {
                cam3.gameObject.SetActive(true);
                cam1.gameObject.SetActive(false);
                cam2.gameObject.SetActive(false);
                //cctv_cam switch sfx here
            }
           
        }
    }


}
