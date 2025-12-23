using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class InventoryUI : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Button closeButton;

    [Header("布局兜底（当 itemListParent 没有 LayoutGroup 时）")]
    [SerializeField] private bool useFallbackLayout = true;
    [SerializeField] private float fallbackRowHeight = 110f;
    [SerializeField] private float fallbackTopPadding = 0f;
    [SerializeField] private bool debugLogBuild = false;
    
    private Inventory inventory;
    private List<GameObject> itemSlotInstances = new List<GameObject>();

    public bool IsOpen => inventoryPanel != null && inventoryPanel.activeSelf;
    
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
        
        // 创建物品槽（按堆叠显示）
        int index = 0;
        foreach (var stack in inventory.Stacks)
        {
            if (stack == null || stack.item == null || stack.count <= 0)
                continue;

            GameObject slotInstance = CreateItemSlot(stack.item, stack.count, index);
            if (slotInstance != null)
            {
                itemSlotInstances.Add(slotInstance);
                index++;
            }
        }

        if (debugLogBuild)
            Debug.Log($"[InventoryUI] Build slots = {itemSlotInstances.Count} (stacks = {inventory.Stacks.Count})");
    }
    
    private GameObject CreateItemSlot(ItemData item, int count, int index)
    {
        if (itemSlotPrefab == null || itemListParent == null) return null;
        
        GameObject slot = Instantiate(itemSlotPrefab, itemListParent);
        ApplyFallbackLayoutIfNeeded(slot, index);
        
        // 查找文本组件显示物品名称
        TMP_Text nameTmp = null;
        foreach (var t in slot.GetComponentsInChildren<TMP_Text>(true))
        {
            // 排除按钮上的文字（例如“使用”）
            if (t.GetComponentInParent<Button>() == null)
            {
                nameTmp = t;
                break;
            }
        }

        var desc = string.IsNullOrWhiteSpace(item.description) ? "" : $"\n{item.description}";
        var line = $"{item.itemName} x{count}\n价值: {item.value}{desc}";

        if (nameTmp != null)
            nameTmp.text = line;
        else
        {
            // 兼容旧的 UGUI Text
            Text nameText = slot.GetComponentInChildren<Text>(true);
            if (nameText != null)
                nameText.text = line;
        }
        
        // 添加使用按钮
        Button useButton = slot.GetComponentInChildren<Button>();
        if (useButton != null)
        {
            // 只有可使用物品才显示按钮，避免沿用预制体里的默认文案（比如“购买”）
            useButton.gameObject.SetActive(item.isUsable);
            if (item.isUsable)
            {
                useButton.onClick.AddListener(() => UseItem(item));

                var buttonTmp = useButton.GetComponentInChildren<TMP_Text>(true);
                if (buttonTmp != null)
                    buttonTmp.text = "使用";
                else
                {
                    // 兼容旧的 UGUI Text
                    var buttonText = useButton.GetComponentInChildren<Text>(true);
                    if (buttonText != null)
                        buttonText.text = "使用";
                }
            }
        }
        
        return slot;
    }

    private void ApplyFallbackLayoutIfNeeded(GameObject slot, int index)
    {
        if (!useFallbackLayout || itemListParent == null || slot == null)
            return;

        // 如果 Content 上有 LayoutGroup，交给 Unity 的布局系统处理
        if (itemListParent.GetComponent<LayoutGroup>() != null)
            return;

        var rt = slot.GetComponent<RectTransform>();
        if (rt == null)
            return;

        // 让每个条目从顶部向下排列，避免全部叠在一起
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -fallbackTopPadding - index * fallbackRowHeight);
        rt.localScale = Vector3.one;
    }
    
    private void UseItem(ItemData item)
    {
        if (inventory != null && item != null)
        {
            SurvivalStats survivalStats = FindObjectOfType<SurvivalStats>();
            if (survivalStats != null && item.itemType == ItemType.Food && item.isUsable)
            {
                survivalStats.RestoreHunger(item.hungerRestore);
                inventory.RemoveItem(item, 1);
            }
        }
    }
    
    // 用于出售物品（由ExchangeStation调用）
    public void OnItemSold(ItemData item)
    {
        if (inventory != null)
        {
            inventory.RemoveItem(item, 1);
        }
    }
}



