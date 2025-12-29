using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ExchangeStation : InteractableBase
{
    [Header("UI引用")]
    [SerializeField] private GameObject exchangePanel;
    [SerializeField] private Transform itemListParent;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button closeButton;

    [Header("布局兜底（当 itemListParent 没有 LayoutGroup 时）")]
    [SerializeField] private bool useFallbackLayout = true;
    [SerializeField] private float fallbackRowHeight = 110f;
    [SerializeField] private float fallbackTopPadding = 0f;
    
    private Inventory inventory;
    private GameManager gameManager;
    private List<GameObject> itemSlotInstances = new List<GameObject>();
    
    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        gameManager = GameManager.Instance;
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseExchangePanel);
        }
        
        if (exchangePanel != null)
        {
            exchangePanel.SetActive(false);
        }
    }
    
    public override void OnInteract()
    {
        if (!CanInteract()) return;
        
        OpenExchangePanel();
    }
    
    public override string GetInteractionPrompt()
    {
        return "按E打开兑换站";
    }
    
    private void OpenExchangePanel()
    {
        if (exchangePanel != null)
        {
            exchangePanel.SetActive(true);
            UpdateMoneyDisplay();
            UpdateItemList();
        }
    }
    
    private void CloseExchangePanel()
    {
        if (exchangePanel != null)
        {
            exchangePanel.SetActive(false);
        }
    }
    
    private void UpdateMoneyDisplay()
    {
        if (moneyText != null && gameManager != null)
        {
            moneyText.text = $"货币: {gameManager.Money}";
        }
    }
    
    private void UpdateItemList()
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
            // 排除按钮上的文字（例如“出售”）
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
        
        // 添加出售按钮
        Button sellButton = slot.GetComponentInChildren<Button>();
        if (sellButton != null)
        {
            sellButton.onClick.AddListener(() => SellItem(item));
            var buttonTmp = sellButton.GetComponentInChildren<TMP_Text>(true);
            if (buttonTmp != null)
                buttonTmp.text = "出售";
            else
            {
                // 兼容旧的 UGUI Text
                Text buttonText = sellButton.GetComponentInChildren<Text>(true);
                if (buttonText != null)
                    buttonText.text = "出售";
            }
        }
        
        return slot;
    }

    private void ApplyFallbackLayoutIfNeeded(GameObject slot, int index)
    {
        if (!useFallbackLayout || itemListParent == null || slot == null)
            return;

        if (itemListParent.GetComponent<LayoutGroup>() != null)
            return;

        var rt = slot.GetComponent<RectTransform>();
        if (rt == null)
            return;

        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -fallbackTopPadding - index * fallbackRowHeight);
        rt.localScale = Vector3.one;
    }
    
    private void SellItem(ItemData item)
    {
        if (inventory == null || gameManager == null || item == null) return;
        
        if (inventory.HasItem(item, 1))
        {
            gameManager.AddMoney(item.value);
            inventory.RemoveItem(item, 1);
            UpdateMoneyDisplay();
            UpdateItemList();
            Debug.Log($"出售 {item.itemName}，获得 {item.value} 货币");
        }
    }
}



