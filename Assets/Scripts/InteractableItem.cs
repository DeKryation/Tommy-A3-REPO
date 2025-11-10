using UnityEngine;

public class InteractableItem : InteractScript
{
    private ItemData item;

    private void Awake()
    {
        item = GetComponent<ItemData>();
    }

    public override void DoOnInteract()
    {
        ItemManager.GetInstance().TryGettingItem(item);
    }

    public override void DoOnRaycastHit()
    {
        GetComponent<Renderer>().material.color = Color.blue;
        Debug.Log("Raycast hit");
    }
}
