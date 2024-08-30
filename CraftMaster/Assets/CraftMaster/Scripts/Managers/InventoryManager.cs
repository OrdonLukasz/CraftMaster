using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static InventoryManager Instance { get; private set; }
    public List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();
    
    [SerializeField] private GameObject[] slots = new GameObject[20];
    [SerializeField] private UnityEvent correctCrafting;
    [SerializeField] private UnityEvent failedCrafting;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject inventoryParent;
    [SerializeField] private GameObject craftingParent;
    
    private GameObject draggedObject;
    private GameObject lastItemSlot;
    private bool isInventoryOpened;
    private bool isCraftingOpened;

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

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        inventoryParent.SetActive(isInventoryOpened);
        craftingParent.SetActive(isCraftingOpened);

        if (draggedObject != null)
        {
            draggedObject.transform.position = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isInventoryOpened)
            {
                Cursor.lockState = CursorLockMode.Locked;

                isInventoryOpened = false;
                isCraftingOpened = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isInventoryOpened = true;
            }
        }
    }

    public List<ItemTypeAndCount> GetAllItems()
    {
        items.Clear();

        foreach (GameObject slot in slots)
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            if (slot != null && slot.heldItem != null)
            {
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                lastItemSlot = clickedObject;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject != null && eventData.pointerCurrentRaycast.gameObject != null &&
            eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            if (slot != null && slot.heldItem == null)
            {
                slot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }
            else if (slot != null && slot.heldItem != null &&
                     slot.heldItem.GetComponent<InventoryItem>().stackCurrent ==
                     slot.heldItem.GetComponent<InventoryItem>().stackMax
                     || slot != null && slot.heldItem != null &&
                     slot.heldItem.GetComponent<InventoryItem>().itemScriptableObject !=
                     draggedObject.GetComponent<InventoryItem>().itemScriptableObject)
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(slot.heldItem);
                slot.heldItem.transform.SetParent(slot.transform.parent.parent.GetChild(2));

                slot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }
            else if (slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().stackCurrent <
                     slot.heldItem.GetComponent<InventoryItem>().stackMax
                     && slot.heldItem.GetComponent<InventoryItem>().itemScriptableObject ==
                     draggedObject.GetComponent<InventoryItem>().itemScriptableObject)
            {
                InventoryItem slotHeldItem = slot.heldItem.GetComponent<InventoryItem>();
                InventoryItem draggedItem = draggedObject.GetComponent<InventoryItem>();

                int itemsToFillStack = slotHeldItem.stackMax - slotHeldItem.stackCurrent;

                if (itemsToFillStack >= draggedItem.stackCurrent)
                {
                    slotHeldItem.stackCurrent += draggedItem.stackCurrent;
                    Destroy(draggedObject);
                }
                else
                {
                    slotHeldItem.stackCurrent += itemsToFillStack;
                    draggedItem.stackCurrent -= itemsToFillStack;
                    lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
                }
            }

            else if (clickedObject.name != "DropItem")
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
                if (slot.transform.parent.parent.GetChild(2) ! != null)
                    draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }

            else
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                Vector3 position = ray.GetPoint(3);
                GameObject newItem =
                    Instantiate(draggedObject.GetComponent<InventoryItem>().itemScriptableObject.prefab, position,
                        new Quaternion());
                newItem.GetComponent<ItemPickable>().itemScriptableObject =
                    draggedObject.GetComponent<InventoryItem>().itemScriptableObject;
                lastItemSlot.GetComponent<InventorySlot>().heldItem = null;
                Destroy(draggedObject);
            }

            draggedObject = null;
        }
    }

    public void ItemPicked(GameObject pickedItem)
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
            newItem.GetComponent<InventoryItem>().itemScriptableObject =
                pickedItem.GetComponent<ItemPickable>().itemScriptableObject;
            newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(2));
            newItem.GetComponent<InventoryItem>().stackCurrent = 1;
            emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
            newItem.transform.localScale = new Vector3(1, 1, 1);
            Destroy(pickedItem);
        }
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
                if (ObjectCraftingPossibilityChance(itemToCraft.item.craftSuccessPercentage))
                {
                    GameObject newItem = Instantiate(itemPrefab);
                    newItem.GetComponent<InventoryItem>().itemScriptableObject = itemToCraft.item;
                    newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(2));
                    newItem.GetComponent<InventoryItem>().stackCurrent = itemToCraft.count;
                    emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
                    newItem.transform.localScale = new Vector3(1, 1, 1);
                    correctCrafting?.Invoke();
                }
                else
                {
                    failedCrafting?.Invoke();
                    Debug.Log("crafting failed");
                }
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

    private bool ObjectCraftingPossibilityChance(float chancePercentage)
    {
        float randomValue = Random.Range(0f, 100f);
        Debug.Log("crafting chance " + randomValue);
        return randomValue <= chancePercentage;
    }

    public void OpenAndCloseCrafting()
    {
        if (isCraftingOpened)
        {
            isCraftingOpened = false;
        }
        else
        {
            isCraftingOpened = true;
        }
    }
}