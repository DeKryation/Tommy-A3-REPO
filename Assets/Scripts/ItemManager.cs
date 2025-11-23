
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ItemManager : MonoBehaviour
{
    private ItemManager() { }

    private static ItemManager instance;

    public static ItemManager GetInstance()
    {
        return instance;
    }

    public static List<ItemData> collectedItems = new List<ItemData>();

    [Header("UI")]
    public RectTransform nametag;
    public RectTransform dialogueBox;
    public static int objNum { get; set; }

    [Header("Inventory")]
    public GameObject invCanvas;
    public Image[] invSlot, itemImages;
    public Sprite emptyInvSlotSprite;
    public Color selectedItemColor;
    public int selectedCanvasSlotID = 0, selectedItemID = -1;

    public Image invGrid;

    public ItemData[] mergedItems;

    public bool dialogueON { get; set; }

    public void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        dialogueON = false;
        collectedItems = new List<ItemData>();
        UpdateInvCanvas();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectItem(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2)) { SelectItem(1); }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { SelectItem(2); }
    }


    private void GiveHintMessage(ItemData item)
    {
            if (item.hintMessage != "")
            {
                StartDialogue(item.hintMessage);
            }
    }

    public void UpdateNameTag(int invNum)
    {
        if (collectedItems.Count > invNum)
        {
            ItemData item = collectedItems[invNum];
            //idk why but item == null doesnt work bcs all destroyed items js becomes null
            if (item.objectName == null)
            {
                Debug.Log("null");
                nametag.gameObject.SetActive(false);
                return;
            }
            nametag.gameObject.SetActive(true);

            string nameText = item.objectName;
            nametag.GetComponentInChildren<TextMeshProUGUI>().text = nameText;
        }
    }

    public void TryGettingItem(ItemData item)
    {
        nametag.gameObject.SetActive(false);

        bool canGetItem = item.reqItemID == -1 || selectedItemID == item.reqItemID;
        // -1 js means no prerequisite
        // the or allows us to pick it up if we equip the prequisite items alr
        if (canGetItem)
        {

            //no merge
            if (!CheckMerge(item.mergeItemID))
            {
                collectedItems.Add(item);
                UpdateInvCanvas();
                if (item.gainMessage != "")
                {
                    StartDialogue(item.gainMessage);
                }
            }
            else
            {
                ItemData a = null;
                for (int i = 0; i < collectedItems.Count; i++)
                {
                    if (collectedItems[i].mergeItemID == item.mergeItemID)
                    {
                        a = collectedItems[i];
                        break;
                    }
                }
                collectedItems.Remove(a);
                collectedItems.Add(mergedItems[item.mergeItemID]);
                UpdateInvCanvas();

                if (mergedItems[item.mergeItemID].gainMessage != "")
                {
                    StartDialogue(mergedItems[item.mergeItemID].gainMessage);
                }
            }
            foreach (GameObject g in item.objectsToRemove)
            {
                Destroy(g);
            }
        }
        else 
            GiveHintMessage(item);
    }


    private bool CheckMerge(int ItemID)
    {
        if (ItemID != -1)
        {
            foreach (ItemData a in collectedItems)
            {
                Debug.Log(a.mergeItemID + " " + ItemID);
                if (a.mergeItemID == ItemID) { return true; }
            } 
        }
        return false;
    }


    public void StartDialogue(string itemMsg)
    {
        StartCoroutine(UpdateDialogueBox(itemMsg));
    }

    public IEnumerator UpdateDialogueBox(string itemMsg)
    {

        if (itemMsg == "")
        {
            dialogueBox.gameObject.SetActive(false);
            yield break;
        }

        yield return new WaitForSeconds(0.1f);

        dialogueBox.gameObject.SetActive(true);

        //change name
        dialogueBox.GetComponentInChildren<TextMeshProUGUI>().text = itemMsg;

        yield return new WaitForSeconds(2f);

        dialogueBox.gameObject.SetActive(false);

    }


    //basically giving hints on how to use item
    public void GetItemInfo(int a)
    {
        if (a != -1)
        {
            Debug.Log(a);
            Debug.Log(collectedItems[selectedCanvasSlotID]);
            ItemData dragged = collectedItems[selectedCanvasSlotID];
            StartDialogue(dragged.hintMessage);

            selectedItemID = -1;
            selectedCanvasSlotID = 0;
        }
    }

    //inventory stuff
    public void SelectItem(int invCanvasID)
    {
        Color c = Color.white;
        //change prev slot to white
        invSlot[selectedCanvasSlotID].color = c;

        //if empty slot
        if (invCanvasID >= collectedItems.Count || invCanvasID < 0)
        {
            //aka nothing selected;
            selectedItemID = -1;
            selectedCanvasSlotID = 0;
            return;
        }
        
        else if (selectedCanvasSlotID == invCanvasID && selectedItemID != -1) 
        {
            //aka deselect if alr selected
            selectedItemID = -1;
            selectedCanvasSlotID = 0;
            nametag.gameObject.SetActive(false);
            return;
        }
        

        //assign the newly selected slots to the variable
        selectedCanvasSlotID = invCanvasID;
        selectedItemID = collectedItems[selectedCanvasSlotID].itemID;
        //change new slot to blu
        UpdateNameTag(selectedCanvasSlotID);
        invSlot[selectedCanvasSlotID].color = selectedItemColor;
    }

    public void UpdateInvCanvas()
    {
        //find out how many items we have
        int itemsAmount = collectedItems.Count, itemSlotAmount = invSlot.Length;
        //replace empty and unused with new image
        for (int i = 0; i < itemSlotAmount; i++)
        {
            //choose between no item and item sprite
            if (i < itemsAmount && collectedItems[i].InvSlotSprite != null)
            {
                itemImages[i].sprite = collectedItems[i].InvSlotSprite;
            }
            else
            {
                itemImages[i].sprite = emptyInvSlotSprite;
            }
        }

        //if dont have any items
        if (itemsAmount == 0)
            SelectItem(0);
    }

}