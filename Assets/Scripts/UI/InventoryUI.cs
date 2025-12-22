using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Button closeButton;
    
    private Inventory inventory;
    private List<GameObject> itemSlotInstances = new List<GameObject>();
    
    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("找不到Inventory组件");
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInventory);
        }
    }
    
    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += UpdateInventoryDisplay;
        }
    }
    
    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateInventoryDisplay;
        }
    }
    
    public void OpenInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            UpdateInventoryDisplay();
        }
    }
    
    public void CloseInventory()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }
    
    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            if (inventoryPanel.activeSelf)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }
    
    private void UpdateInventoryDisplay()
    {
        if (itemListParent == null || inventory == null) return;
        
        // 清除现有显示
        foreach (GameObject slot in itemSlotInstances)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }
        itemSlotInstances.Clear();
        
        // 创建物品槽
        foreach (ItemData item in inventory.Items)
        {
            GameObject slotInstance = CreateItemSlot(item);
            if (slotInstance != null)
            {
                itemSlotInstances.Add(slotInstance);
            }
        }
    }
    
    private GameObject CreateItemSlot(ItemData item)
    {
        if (itemSlotPrefab == null || itemListParent == null) return null;
        
        GameObject slot = Instantiate(itemSlotPrefab, itemListParent);
        
        // 查找文本组件显示物品名称
        Text nameText = slot.GetComponentInChildren<Text>();
        if (nameText != null)
        {
            nameText.text = $"{item.itemName} (价值: {item.value})";
        }
        
        // 添加使用按钮
        Button useButton = slot.GetComponentInChildren<Button>();
        if (useButton != null && item.isUsable)
        {
            useButton.onClick.AddListener(() => UseItem(item));
            useButton.GetComponentInChildren<Text>().text = "使用";
        }
        
        return slot;
    }
    
    private void UseItem(ItemData item)
    {
        if (inventory != null && item != null)
        {
            SurvivalStats survivalStats = FindObjectOfType<SurvivalStats>();
            if (survivalStats != null && item.itemType == ItemType.Food && item.isUsable)
            {
                survivalStats.RestoreHunger(item.hungerRestore);
                inventory.RemoveItem(item);
            }
        }
    }
    
    // 用于出售物品（由ExchangeStation调用）
    public void OnItemSold(ItemData item)
    {
        if (inventory != null)
        {
            inventory.RemoveItem(item);
        }
    }
}


