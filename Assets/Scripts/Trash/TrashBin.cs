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
    [Header("配置来源")]
    [Tooltip("所有数值从 GameBalanceConfig 读取")]
    [SerializeField] private bool useGameBalanceConfig = true;
    
    [Header("掉落表")]
    [SerializeField] private List<TrashDrop> dropTable = new List<TrashDrop>();
    
    private bool isSearching = false;
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private Inventory inventory;
    private SurvivalStats survivalStats;
    
    // 缓存配置
    private GameBalanceConfig config;
    
    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        survivalStats = FindObjectOfType<SurvivalStats>();
        
        if (inventory == null)
        {
            Debug.LogError("找不到Inventory组件");
        }
        
        // 加载配置
        if (useGameBalanceConfig && GameBalance.Config != null)
        {
            config = GameBalance.Config;
            Debug.Log("[TrashBin] 已从 GameBalanceConfig 加载配置");
        }
        else
        {
            Debug.LogWarning("[TrashBin] 未找到 GameBalanceConfig，使用默认值");
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
        
        // 从配置读取翻找时间
        float searchDuration = config != null ? config.binSearchTime : 2.5f;
        
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
        
        // 从配置读取风险概率和伤害
        bool hasRisk = config != null ? config.RollBinRisk() : (Random.value < 0.15f);
        float damageAmount = config != null ? config.binRiskDamage : 10f;
        
        // 有概率降低健康度
        if (hasRisk && survivalStats != null)
        {
            survivalStats.ReduceHealth(damageAmount);
            Debug.Log($"翻找垃圾桶时受了伤，健康度 -{damageAmount}");
        }
        
        // 从配置读取冷却时间
        float cooldownDuration = config != null ? config.binCooldown : 150f;
        
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
        
        // 从配置获取掉落数量
        int lootCount = config != null ? config.GetBinLootCount() : Random.Range(2, 5);
        
        // 根据掉落表随机掉落物品（限制数量）
        int droppedCount = 0;
        List<TrashDrop> shuffledDrops = new List<TrashDrop>(dropTable);
        
        // 打乱顺序以增加随机性
        for (int i = 0; i < shuffledDrops.Count; i++)
        {
            TrashDrop temp = shuffledDrops[i];
            int randomIndex = Random.Range(i, shuffledDrops.Count);
            shuffledDrops[i] = shuffledDrops[randomIndex];
            shuffledDrops[randomIndex] = temp;
        }
        
        foreach (TrashDrop drop in shuffledDrops)
        {
            if (droppedCount >= lootCount) break;
            
            if (drop.item != null && Random.value <= drop.dropChance)
            {
                if (inventory.AddItem(drop.item))
                {
                    Debug.Log($"从垃圾桶找到: {drop.item.itemName}");
                    droppedCount++;
                }
                else
                {
                    Debug.Log("背包已满，物品掉落在地上");
                    // 这里可以实例化一个地面物品
                }
            }
        }
        
        Debug.Log($"[TrashBin] 本次掉落 {droppedCount} 个物品");
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



