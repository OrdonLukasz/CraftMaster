using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private InventoryManager inventoryManager;

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 3))
            {
                ItemPickable item = hitInfo.collider.gameObject.GetComponent<ItemPickable>();

                if (item != null)
                {
                    inventoryManager.ItemPicked(hitInfo.collider.gameObject);
                }
            }
        }
    }
}