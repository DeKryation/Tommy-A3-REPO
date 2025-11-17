using UnityEngine;
using UnityEngine.UI;

public class CCTV_Controller : InteractScript
{
    public Camera playerCam;
    public LayerMask mask;
    public LayerMask mask2;

    public Camera mouseCam;
    public GameObject CCTV_Cam;
    public GameObject CCTV_Buttons;

    public GameObject from_CCTV;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //RayCast();

        if(Input.GetMouseButtonDown(0))
        {
            DoOnInteract();
            OnClick_Back();
        }
        
    }



    public override void DoOnInteract()
    {
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 2f);


        if (Physics.Raycast(ray, out hit, 5, mask))
        {
            Debug.Log("Hit: " + hit.collider.name);

            playerCam.gameObject.SetActive(false);
            CCTV_Buttons.SetActive(true);
            CCTV_Cam.SetActive(true);
            from_CCTV.gameObject.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }

    }

    public void OnClick_Back()
    {
        Ray ray = mouseCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 150, mask2))
        {
            if(hit.transform.name == "BackButton")
            {
                playerCam.gameObject.SetActive(true);

                CCTV_Buttons.SetActive(false);
                CCTV_Cam.SetActive(false);
                from_CCTV.gameObject.SetActive(false);

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                Debug.Log("BACK");
            }
        }


    }
}
