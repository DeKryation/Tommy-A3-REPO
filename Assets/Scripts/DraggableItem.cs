using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image image;
    public int invNum;
    public Canvas canvas;
    public static int usedYet;
    [HideInInspector] public Transform parentAfterDrag;


    //when start drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        usedYet = ItemManager.collectedItems.Count;
        //assigns original parent to parent after drag
        parentAfterDrag = transform.parent;
        //right as we start the drag, the parent switches to the canvas which is where everything is under
        transform.SetParent(canvas.transform);
        //set as last sibling aka making it the top opject in the layer bcs last is usually on top
        transform.SetAsLastSibling();

        //when start drag item is invis under mouse (to mouse input)
        image.raycastTarget = false;

    }

    //dragging
    public void OnDrag(PointerEventData eventData)
    {
        //movement follows mouse
        transform.position = Input.mousePosition;
    }

    //when end drag
    public void OnEndDrag(PointerEventData eventData)
    {
        ItemManager.GetInstance().GetItemInfo(invNum);

        //reassigns when done dragging
        transform.SetParent(parentAfterDrag);

        //no longer invis
        image.raycastTarget = true;

    }

    //if it gets dropped
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();

        //draggableItem.parentAfterDrag = transform;

        //this is the thing we were dragging around
        ItemData itemA = ItemManager.collectedItems[draggableItem.invNum];

        //this is the item it was dropped on
        ItemData itemB = ItemManager.collectedItems[invNum];

        if (itemA.mergeItemID != -1 && itemB.mergeItemID == itemA.mergeItemID)
        {
            ItemData itemX = ItemManager.GetInstance().mergedItems[itemA.mergeItemID];

            switch (itemB.mergeItemID)
            {
                case 0:
                case 1:
                    ItemManager.collectedItems.Remove(itemA);
                    ItemManager.collectedItems.Remove(itemB);
                    if (invNum >= draggableItem.invNum)
                    {
                        ItemManager.collectedItems.Insert(draggableItem.invNum, itemX);
                        ItemManager.GetInstance().UpdateNameTag(draggableItem.invNum);
                    }
                    else
                    {
                        ItemManager.collectedItems.Insert(invNum, itemX);
                        ItemManager.GetInstance().UpdateNameTag(invNum);
                    }
                    ItemManager.GetInstance().UpdateInvCanvas();
                    break;
                default:
                    Debug.Log("unknown merge combination found");
                    break;
            }

        }
    }
}