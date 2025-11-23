using UnityEngine;

public class ItemData : MonoBehaviour
{
    public int itemID, reqItemID, mergeItemID;

    /* 
    REQ ITEM ID
    -1 - Can be picked up or interacted with
    99 - not meant to be picked up and just trigers dialogue

    ITEM ID
    0 - Screwdriver Head
    1 - Screwdriver Tip
    2 - Screwdriver

    MERGE ITEM ID
    -1 -- cant be combined
    */
    public string objectName;

    public GameObject[] objectsToRemove;
    [TextArea(2, 2)]
    public string hintMessage;
    //scene transition messages and failed to pick up item messages
    [TextArea(2, 2)]
    public string gainMessage;
    //item pickup messages

    public Sprite InvSlotSprite;

}