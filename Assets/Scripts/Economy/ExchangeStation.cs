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

        if (nameTmp != null)
            nameTmp.text = $"{item.itemName} - {item.value} 货币";
        else
        {
            // 兼容旧的 UGUI Text
            Text nameText = slot.GetComponentInChildren<Text>(true);
            if (nameText != null)
                nameText.text = $"{item.itemName} - {item.value} 货币";
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
    
    private void SellItem(ItemData item)
    {
        if (inventory == null || gameManager == null || item == null) return;
        
        if (inventory.HasItem(item))
        {
            gameManager.AddMoney(item.value);
            inventory.RemoveItem(item);
            UpdateMoneyDisplay();
            UpdateItemList();
            Debug.Log($"出售 {item.itemName}，获得 {item.value} 货币");
        }
    }
}


