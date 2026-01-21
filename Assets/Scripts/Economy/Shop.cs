using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class ShopItem
{
    public ItemData item;
    public int price;
}

public class Shop : InteractableBase
{
    [Header("商店设置")]
    [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
    
    [Header("UI引用")]
    [SerializeField] private GameObject shopPanel;
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
            closeButton.onClick.AddListener(CloseShopPanel);
        }
        
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }
    
    public override void OnInteract()
    {
        if (!CanInteract()) return;
        
        OpenShopPanel();
    }
    
    public override string GetInteractionPrompt()
    {
        return "按E打开商店";
    }
    
    private void OpenShopPanel()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            UpdateMoneyDisplay();
            UpdateShopItemList();
        }
    }
    
    private void CloseShopPanel()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }
    
    private void UpdateMoneyDisplay()
    {
        if (moneyText != null && gameManager != null)
        {
            moneyText.text = $"货币: {gameManager.Money}";
        }
    }
    
    private void UpdateShopItemList()
    {
        if (itemListParent == null) return;
        
        // 清除现有显示
        foreach (GameObject slot in itemSlotInstances)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }
        itemSlotInstances.Clear();
        
        // 创建商品槽
        int index = 0;
        foreach (ShopItem shopItem in shopItems)
        {
            if (shopItem.item != null)
            {
                GameObject slotInstance = CreateShopItemSlot(shopItem, index);
                if (slotInstance != null)
                {
                    itemSlotInstances.Add(slotInstance);
                    index++;
                }
            }
        }
    }
    
    private GameObject CreateShopItemSlot(ShopItem shopItem, int index)
    {
        if (itemSlotPrefab == null || itemListParent == null) return null;
        
        GameObject slot = Instantiate(itemSlotPrefab, itemListParent);
        ApplyFallbackLayoutIfNeeded(slot, index);
        
        // 查找文本组件显示物品名称
        TMP_Text nameTmp = null;
        foreach (var t in slot.GetComponentsInChildren<TMP_Text>(true))
        {
            // 排除按钮上的文字（例如“购买”）
            if (t.GetComponentInParent<Button>() == null)
            {
                nameTmp = t;
                break;
            }
        }

        if (nameTmp != null)
            nameTmp.text = $"{shopItem.item.itemName} - {shopItem.price} 货币";
        else
        {
            // 兼容旧的 UGUI Text
            Text nameText = slot.GetComponentInChildren<Text>(true);
            if (nameText != null)
                nameText.text = $"{shopItem.item.itemName} - {shopItem.price} 货币";
        }
        
        // 添加购买按钮
        Button buyButton = slot.GetComponentInChildren<Button>();
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(() => BuyItem(shopItem));
            var buttonTmp = buyButton.GetComponentInChildren<TMP_Text>(true);
            if (buttonTmp != null)
                buttonTmp.text = "购买";
            else
            {
                // 兼容旧的 UGUI Text
                Text buttonText = buyButton.GetComponentInChildren<Text>(true);
                if (buttonText != null)
                    buttonText.text = "购买";
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
    
    private void BuyItem(ShopItem shopItem)
    {
        if (inventory == null || gameManager == null || shopItem.item == null) return;
        
        if (gameManager.Money >= shopItem.price)
        {
            if (inventory.AddItem(shopItem.item))
            {
                gameManager.SpendMoney(shopItem.price);
                UpdateMoneyDisplay();
                Debug.Log($"购买了 {shopItem.item.itemName}，花费 {shopItem.price} 货币");
            }
            else
            {
                Debug.Log("背包已满，无法购买");
            }
        }
        else
        {
            Debug.Log("货币不足");
        }
    }
}





