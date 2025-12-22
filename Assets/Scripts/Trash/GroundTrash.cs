using UnityEngine;

public class GroundTrash : InteractableBase
{
    [Header("垃圾设置")]
    [SerializeField] private ItemData trashItem;
    
    private Inventory inventory;
    
    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("找不到Inventory组件");
        }
    }
    
    public override void OnInteract()
    {
        if (!CanInteract()) return;
        
        if (inventory != null && trashItem != null)
        {
            if (inventory.AddItem(trashItem))
            {
                Debug.Log($"拾取了: {trashItem.itemName}");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("背包已满，无法拾取");
            }
        }
    }
    
    public override string GetInteractionPrompt()
    {
        if (trashItem != null)
        {
            return $"按E拾取 {trashItem.itemName}";
        }
        return "按E拾取";
    }
    
    // 设置垃圾物品（由TrashSpawner调用）
    public void SetTrashItem(ItemData item)
    {
        trashItem = item;
    }
}


