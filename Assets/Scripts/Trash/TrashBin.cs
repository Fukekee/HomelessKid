using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TrashDrop
{
    public ItemData item;
    public float dropChance = 1f; // 掉落概率（0-1）
}

public class TrashBin : InteractableBase
{
    [Header("翻找设置")]
    [SerializeField] private float searchDuration = 3f; // 翻找所需时间（秒）
    [SerializeField] private float cooldownDuration = 10f; // 冷却时间（秒）
    [SerializeField] private float healthDamageChance = 0.3f; // 降低健康度的概率
    [SerializeField] private float healthDamageAmount = 10f; // 降低的健康度数值
    
    [Header("掉落表")]
    [SerializeField] private List<TrashDrop> dropTable = new List<TrashDrop>();
    
    private bool isSearching = false;
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private Inventory inventory;
    private SurvivalStats survivalStats;
    
    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        survivalStats = FindObjectOfType<SurvivalStats>();
        
        if (inventory == null)
        {
            Debug.LogError("找不到Inventory组件");
        }
    }
    
    private void Update()
    {
        base.Update();
        
        // 更新冷却时间
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
            }
        }
    }
    
    public override void OnInteract()
    {
        if (!CanInteract()) return;
        
        if (isSearching)
        {
            return;
        }
        
        if (isOnCooldown)
        {
            Debug.Log($"垃圾桶还在冷却中，还需等待 {cooldownTimer:F1} 秒");
            return;
        }
        
        StartCoroutine(SearchTrashBin());
    }
    
    private IEnumerator SearchTrashBin()
    {
        isSearching = true;
        
        // 显示进度条（如果有UIManager）
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowProgressBar(searchDuration);
        }
        
        Debug.Log("开始翻找垃圾桶...");
        
        // 等待翻找时间
        float elapsed = 0f;
        while (elapsed < searchDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 翻找完成，掉落物品
        DropItems();
        
        // 有概率降低健康度
        if (Random.value < healthDamageChance && survivalStats != null)
        {
            survivalStats.ReduceHealth(healthDamageAmount);
            Debug.Log($"翻找垃圾桶时受了伤，健康度 -{healthDamageAmount}");
        }
        
        // 开始冷却
        isOnCooldown = true;
        cooldownTimer = cooldownDuration;
        
        isSearching = false;
        
        if (uiManager != null)
        {
            uiManager.HideProgressBar();
        }
    }
    
    private void DropItems()
    {
        if (inventory == null || dropTable.Count == 0) return;
        
        // 根据掉落表随机掉落物品
        foreach (TrashDrop drop in dropTable)
        {
            if (drop.item != null && Random.value <= drop.dropChance)
            {
                if (inventory.AddItem(drop.item))
                {
                    Debug.Log($"从垃圾桶找到: {drop.item.itemName}");
                }
                else
                {
                    Debug.Log("背包已满，物品掉落在地上");
                    // 这里可以实例化一个地面物品
                }
            }
        }
    }
    
    public override string GetInteractionPrompt()
    {
        if (isOnCooldown)
        {
            return $"垃圾桶冷却中 ({cooldownTimer:F1}s)";
        }
        
        if (isSearching)
        {
            return "正在翻找...";
        }
        
        return "按E翻找垃圾桶";
    }
    
    public override bool CanInteract()
    {
        return base.CanInteract() && !isSearching && !isOnCooldown;
    }
}


