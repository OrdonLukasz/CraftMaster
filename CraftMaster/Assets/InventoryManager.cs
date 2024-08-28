using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();
    [SerializeField] GameObject[] slots = new GameObject[20];

    [SerializeField] GameObject itemPrefab;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public List<ItemTypeAndCount> GetAllItems()
    {
        items.Clear();

        foreach(GameObject slot in slots)
        {
            InventorySlot slotScript = slot.GetComponent<InventorySlot>();

            if (slotScript.heldItem != null)
            {
                InventoryItem itemScript = slotScript.heldItem.GetComponent<InventoryItem>();

                int i = 0;
                bool wasItemAdded = false;

                foreach (ItemTypeAndCount itemAndCount in items)
                {
                    if (itemAndCount.item == itemScript.itemScriptableObject)
                    {
                        items[i].count += itemScript.stackCurrent;
                        wasItemAdded = true;
                    }
                    i++;
                }

                if (!wasItemAdded)
                {
                    items.Add(new ItemTypeAndCount(itemScript.itemScriptableObject, itemScript.stackCurrent));
                }
            }
        }

        return items;
    }
    
     public void CraftItems(List<ItemTypeAndCount> itemsToCraft, List<ItemTypeAndCount> itemsToDestroy)
    {
        foreach (ItemTypeAndCount itemToCraft in itemsToCraft)
        {
            GameObject emptySlot = null;

            for (int i = 0; i < slots.Length; i++)
            {
                InventorySlot slot = slots[i].GetComponent<InventorySlot>();

                if (slot.heldItem == null)
                {
                    emptySlot = slots[i];
                    break;
                }
            }

            if (emptySlot != null)
            {
                GameObject newItem = Instantiate(itemPrefab);
                newItem.GetComponent<InventoryItem>().itemScriptableObject = itemToCraft.item;
                newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(2));
                newItem.GetComponent<InventoryItem>().stackCurrent = itemToCraft.count;

                emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
                newItem.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetComponent<InventorySlot>().heldItem != null)
            {
                InventorySlot slotScript = slots[i].GetComponent<InventorySlot>();
                InventoryItem itemScript = slotScript.heldItem.GetComponent<InventoryItem>();

                foreach (ItemTypeAndCount itemToDestroy in itemsToDestroy)
                {
                    if (itemToDestroy.item == itemScript.itemScriptableObject)
                    {
                        if (itemToDestroy.count >= itemScript.stackCurrent)
                        {
                            slotScript.heldItem = null;
                            Destroy(itemScript.gameObject);
                        }
                        else if (itemToDestroy.count < itemScript.stackCurrent)
                        {
                            itemScript.stackCurrent -= itemToDestroy.count;
                        }
                    }
                }
            }
        }
    }
}