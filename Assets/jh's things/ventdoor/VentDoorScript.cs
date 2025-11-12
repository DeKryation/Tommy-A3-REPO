using UnityEngine;

public class VentDoorScript : InteractScript
{
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public override void DoOnInteract()
    {
        if (ItemManager.GetInstance().selectedItemID == 3) //ID for screwdriver is 3!!
        {
            anim.SetBool("ventDoorUnlocked", true);
        }
    }

    public override void DoOnRaycastHit()
    {
        Debug.Log("Help");
    }
}
