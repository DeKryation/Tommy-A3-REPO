using UnityEngine;

public class door : InteractScript
{
    public override void DoOnInteract()
    {
        Debug.LogWarning("door interacted");
        if (ItemManager.GetInstance().selectedItemID == 4) //ID for key
        {
            SceneTransition.GetInstance().ChangeScene("Lucas' Door QTE");
        }
    }
}
